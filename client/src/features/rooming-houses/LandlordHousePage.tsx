import { useEffect, useMemo, useState } from "react";
import type { ReactNode } from "react";
import { getApiErrorMessage } from "../../shared/api/apiError";
import "./LandlordHousePage.css";
import {
  getAmenities,
  getMyRoomingHouses,
  getRoomingHouseDetail,
} from "./api";
import {
  createRoom,
  getRoomDetail,
  getRoomsByRoomingHouse,
  submitRoom,
  updateRoom,
  updateRoomAmenities,
  updateRoomImages,
  updateRoomPriceTiers,
} from "../rooms/api";
import type {
  Amenity,
  PropertyImageRequest,
  RoomingHouseDetail,
  RoomingHouseSummary,
} from "./types";
import type { CreateRoomRequest, Room, RoomPriceTierRequest } from "../rooms/types";
import RoomingHouseEditor from "./components/RoomingHouseEditor";
import PropertyImageEditor from "./components/PropertyImageEditor";

type DashboardMode =
  | "houses"
  | "rooms"
  | "edit-house"
  | "create-house"
  | "room-detail"
  | "edit-room"
  | "create-room";
type RoomTab = "basic" | "images" | "amenities" | "price";

const emptyRoomForm: CreateRoomRequest = {
  roomNumber: "",
  floor: 1,
  areaM2: null,
  maxOccupants: 1,
  description: "",
};

export default function LandlordHousePage() {
  const [houses, setHouses] = useState<RoomingHouseSummary[]>([]);
  const [rooms, setRooms] = useState<Room[]>([]);
  const [selectedHouse, setSelectedHouse] = useState<RoomingHouseDetail | null>(null);
  const [selectedRoom, setSelectedRoom] = useState<Room | null>(null);
  const [roomAmenities, setRoomAmenities] = useState<Amenity[]>([]);
  const [mode, setMode] = useState<DashboardMode>("houses");
  const [roomTab, setRoomTab] = useState<RoomTab>("basic");
  const [loading, setLoading] = useState(true);
  const [sectionLoading, setSectionLoading] = useState(false);
  const [message, setMessage] = useState("");

  const [roomForm, setRoomForm] = useState<CreateRoomRequest>(emptyRoomForm);
  const [roomImages, setRoomImages] = useState<PropertyImageRequest[]>([]);
  const [roomAmenityIds, setRoomAmenityIds] = useState<number[]>([]);
  const [priceTiers, setPriceTiers] = useState<RoomPriceTierRequest[]>([]);

  useEffect(() => {
    loadHouses();
  }, []);

  const houseStats = useMemo(() => {
    return {
      total: houses.length,
      approved: houses.filter((house) => house.approvalStatus === "Approved").length,
      pending: houses.filter((house) => house.approvalStatus === "Pending").length,
      rejected: houses.filter((house) => house.approvalStatus === "Rejected").length,
    };
  }, [houses]);

  const roomStats = useMemo(() => {
    return {
      total: rooms.length,
      available: rooms.filter((room) => room.status === "Available").length,
      occupied: rooms.filter((room) => room.status === "Occupied").length,
      hidden: rooms.filter((room) => room.status === "Hidden").length,
    };
  }, [rooms]);

  async function loadHouses() {
    setLoading(true);
    setMessage("");

    try {
      const data = await getMyRoomingHouses();
      setHouses(data);
      setMode("houses");
      setSelectedHouse(null);
      setSelectedRoom(null);
      setRooms([]);
    } catch (error) {
      setMessage(getApiErrorMessage(error, "Không thể tải danh sách khu trọ."));
    } finally {
      setLoading(false);
    }
  }

  async function openHouse(houseId: string) {
    setSectionLoading(true);
    setMessage("");

    try {
      const houseDetail = await getRoomingHouseDetail(houseId);
      setSelectedHouse(houseDetail);
      setSelectedRoom(null);

      if (houseDetail.approvalStatus === "Approved") {
        const roomList = await getRoomsByRoomingHouse(houseId);
        setRooms(roomList);
        setMode("rooms");
        return;
      }

      setRooms([]);

      if (houseDetail.approvalStatus === "Draft" || houseDetail.approvalStatus === "Rejected") {
        setMode("edit-house");
        return;
      }

      setMode("rooms");
    } catch (error) {
      setMessage(getApiErrorMessage(error, "Không thể tải thông tin khu trọ."));
    } finally {
      setSectionLoading(false);
    }
  }

  async function openRoom(roomId: string) {
    setSectionLoading(true);
    setMessage("");

    try {
      const roomDetail = await getRoomDetail(roomId);
      setSelectedRoom(roomDetail);
      setMode("room-detail");
    } catch (error) {
      setMessage(getApiErrorMessage(error, "Không thể tải thông tin phòng."));
    } finally {
      setSectionLoading(false);
    }
  }

  async function openHouseEditor() {
    if (!selectedHouse) {
      return;
    }

    setMode("edit-house");
  }

  function openCreateHouse() {
    const blockingHouse = houses.find((house) =>
      house.approvalStatus === "Draft" ||
      house.approvalStatus === "Pending" ||
      house.approvalStatus === "Rejected",
    );

    if (blockingHouse) {
      setSelectedHouse(null);
      setSelectedRoom(null);
      setRooms([]);
      setMode("houses");
      setMessage(getCreateHouseBlockedMessage(blockingHouse.approvalStatus));
      return;
    }

    setSelectedHouse(null);
    setSelectedRoom(null);
    setRooms([]);
    setMessage("");
    setMode("create-house");
  }

  async function openRoomEditor() {
    if (!selectedRoom) {
      return;
    }

    setRoomTab("basic");
    setRoomForm({
      roomNumber: selectedRoom.roomNumber,
      floor: selectedRoom.floor,
      areaM2: selectedRoom.areaM2 ?? null,
      maxOccupants: selectedRoom.maxOccupants,
      description: selectedRoom.description ?? "",
    });
    setRoomImages(toImageRequests(selectedRoom.images));
    setRoomAmenityIds(selectedRoom.amenities.map((amenity) => amenity.id));
    setPriceTiers(
      selectedRoom.priceTiers.map((tier) => ({
        occupantCount: tier.occupantCount,
        monthlyRent: tier.monthlyRent,
        isActive: tier.isActive,
      })),
    );

    try {
      if (roomAmenities.length === 0) {
        setRoomAmenities(await getAmenities("Room"));
      }

      setMode("edit-room");
    } catch (error) {
      setMessage(
        getApiErrorMessage(error, "Không thể tải danh mục tiện ích phòng."),
      );
    }
  }

  async function openCreateRoom() {
    setSelectedRoom(null);
    setRoomTab("basic");
    setRoomForm(emptyRoomForm);
    setRoomImages([]);
    setRoomAmenityIds([]);
    setPriceTiers([{ occupantCount: 1, monthlyRent: 0, isActive: true }]);
    setMessage("");

    try {
      if (roomAmenities.length === 0) {
        setRoomAmenities(await getAmenities("Room"));
      }

      setMode("create-room");
    } catch (error) {
      setMessage(
        getApiErrorMessage(error, "Không thể tải danh mục tiện ích phòng."),
      );
    }
  }

  async function saveRoomBasic() {
    if (!selectedRoom && (mode !== "create-room" || !selectedHouse)) {
      return;
    }

    await saveRoomChange(async () => {
      if (selectedRoom) {
        return updateRoom(selectedRoom.id, roomForm);
      }

      const createdRoom = await createRoom(selectedHouse!.id, roomForm);
      setRoomTab("images");
      return createdRoom;
    });
  }

  async function saveRoomImages() {
    if (!selectedRoom) {
      return;
    }

    await saveRoomChange(() =>
      updateRoomImages(selectedRoom.id, cleanImages(roomImages)),
    );
  }

  async function saveRoomAmenities() {
    if (!selectedRoom) {
      return;
    }

    await saveRoomChange(() => updateRoomAmenities(selectedRoom.id, roomAmenityIds));
  }

  async function savePriceTiers() {
    if (!selectedRoom) {
      return;
    }

    await saveRoomChange(() => updateRoomPriceTiers(selectedRoom.id, priceTiers));
  }

  async function publishSelectedRoom() {
    if (!selectedRoom) {
      return;
    }

    await saveRoomChange(() => submitRoom(selectedRoom.id));
  }

  async function saveRoomChange(request: () => Promise<Room>) {
    setSectionLoading(true);
    setMessage("");

    try {
      const updatedRoom = await request();
      setSelectedRoom(updatedRoom);
      setRooms((current) =>
        current.some((room) => room.id === updatedRoom.id)
          ? current.map((room) => (room.id === updatedRoom.id ? updatedRoom : room))
          : [...current, updatedRoom],
      );
      setMessage("Đã lưu thông tin phòng.");
    } catch (error) {
      setMessage(getApiErrorMessage(error, "Không thể lưu thông tin phòng."));
    } finally {
      setSectionLoading(false);
    }
  }

  function renderOverview() {
    if (mode === "houses" || !selectedHouse) {
      return (
        <Overview
          title="Khu trọ của tôi"
          subtitle="Quản lý các khu trọ của bạn"
          stats={[
            ["Tổng khu trọ", houseStats.total],
            ["Đã duyệt", houseStats.approved],
            ["Chờ duyệt", houseStats.pending],
            ["Bị từ chối", houseStats.rejected],
          ]}
          actions={
            <button className="primary-action" onClick={openCreateHouse}>
              Tạo khu trọ mới
            </button>
          }
        />
      );
    }

    if ((mode === "room-detail" || mode === "edit-room") && selectedRoom) {
      return (
        <Overview
          title={`Phòng ${selectedRoom.roomNumber}`}
          subtitle={selectedHouse.name}
          stats={[
            ["Trạng thái", formatStatus(selectedRoom.status)],
            ["Tầng", selectedRoom.floor],
            ["Diện tích", selectedRoom.areaM2 ? `${selectedRoom.areaM2} m²` : "Chưa có"],
            ["Tối đa", selectedRoom.maxOccupants],
          ]}
          actions={
            mode === "room-detail" ? (
              <>
                {selectedRoom.status === "Hidden" && (
                  <button onClick={publishSelectedRoom}>Hiển thị phòng</button>
                )}
                <button onClick={openRoomEditor}>Chỉnh sửa phòng</button>
              </>
            ) : null
          }
        />
      );
    }

    const canManageRooms = selectedHouse.approvalStatus === "Approved";

    return (
      <Overview
        title={selectedHouse.name}
        subtitle={selectedHouse.addressDisplay}
        stats={[
          ["Tổng phòng", roomStats.total],
          ["Còn trống", roomStats.available],
          ["Đang thuê", roomStats.occupied],
          ["Đang ẩn", roomStats.hidden],
        ]}
        actions={
          <>
            {selectedHouse.approvalStatus !== "Pending" && (
              <button onClick={openHouseEditor}>Chỉnh sửa khu trọ</button>
            )}
            {canManageRooms && (
              <button
                className="primary-action"
                onClick={openCreateRoom}
              >
                Tạo phòng mới
              </button>
            )}
          </>
        }
      />
    );
  }

  function renderContent() {
    if (loading) {
      return <div className="empty-panel">Đang tải bảng quản lý...</div>;
    }

    if (mode === "houses") {
      return (
        <section className="card-grid">
          {houses.length === 0 ? (
            <div className="empty-panel">
              <h2>Bạn chưa có khu trọ nào</h2>
              <p>Tạo khu trọ đầu tiên để bắt đầu quản lý phòng cho thuê.</p>
            </div>
          ) : (
            houses.map((house) => (
              <button
                className="dashboard-card"
                key={house.id}
                onClick={() => openHouse(house.id)}
              >
                <CardHeader title={house.name} meta={formatDate(house.updatedAt)} />
                <p>{house.addressDisplay}</p>
                <StatusRow
                  approvalStatus={house.approvalStatus}
                  visibilityStatus={house.visibilityStatus}
                />
                {house.rejectedReason && (
                  <p className="warning-text">{house.rejectedReason}</p>
                )}
              </button>
            ))
          )}
        </section>
      );
    }

    if (mode === "rooms" && selectedHouse) {
      return (
        <>
          <button className="back-link" onClick={loadHouses}>
            Quay lại danh sách khu trọ
          </button>
          <section className="card-grid">
            {selectedHouse.approvalStatus === "Pending" ? (
              <div className="empty-panel">
                <h2>Khu trọ đang chờ duyệt</h2>
                <p>Bạn có thể quản lý phòng sau khi khu trọ được quản trị viên phê duyệt.</p>
              </div>
            ) : rooms.length === 0 ? (
              <div className="empty-panel">
                <h2>Chưa có phòng nào</h2>
                <p>Tạo phòng đầu tiên cho khu trọ này.</p>
              </div>
            ) : (
              rooms.map((room) => (
                <button
                  className="dashboard-card"
                  key={room.id}
                  onClick={() => openRoom(room.id)}
                >
                  <CardHeader title={`Phòng ${room.roomNumber}`} meta={formatDate(room.updatedAt)} />
                  <p>Tầng {room.floor}</p>
                  <p>{room.areaM2 ? `${room.areaM2} m²` : "Chưa có diện tích"} - tối đa {room.maxOccupants} người</p>
                  <span className={`status-pill ${getStatusToneClass(room.status)}`}>
                    {formatStatus(room.status)}
                  </span>
                </button>
              ))
            )}
          </section>
        </>
      );
    }

    if (mode === "edit-house") {
      return renderHouseEditor();
    }

    if (mode === "create-house") {
      return renderCreateHouseEditor();
    }

    if (mode === "room-detail" && selectedRoom) {
      return (
        <>
          <button className="back-link" onClick={() => setMode("rooms")}>
            Quay lại danh sách phòng
          </button>
          <section className="empty-panel">
            <h2>Danh sách người đang ở</h2>
            <p>Tính năng này sẽ được triển khai ở giai đoạn sau.</p>
          </section>
        </>
      );
    }

    if (mode === "edit-room" || mode === "create-room") {
      return renderRoomEditor();
    }

    return null;
  }

  function renderHouseEditor() {
    if (!selectedHouse) {
      return null;
    }

    const canManageRooms = selectedHouse.approvalStatus === "Approved";
    const canSubmitHouseForReview =
      selectedHouse.approvalStatus === "Draft" ||
      selectedHouse.approvalStatus === "Rejected";

    return (
      <section className="editor-panel editor-panel--embedded">
        <button
          className="back-link"
          onClick={() => {
            if (canManageRooms) {
              setMode("rooms");
              return;
            }

            loadHouses();
          }}
        >
          {canManageRooms ? "Quay lại danh sách phòng" : "Quay lại danh sách khu trọ"}
        </button>
        <RoomingHouseEditor
          title="Chỉnh sửa khu trọ"
          subtitle="Cập nhật thông tin, ảnh, tiện ích và giấy tờ pháp lý của khu trọ."
          initialRoomingHouse={selectedHouse}
          allowSubmit={canSubmitHouseForReview}
          submitLabel={
            selectedHouse.approvalStatus === "Rejected"
              ? "Gửi duyệt lại"
              : "Gửi duyệt"
          }
          onChange={(updatedHouse) => {
            setSelectedHouse(updatedHouse);
            setHouses((current) =>
              current.map((house) =>
                house.id === updatedHouse.id ? updatedHouse : house,
              ),
            );
          }}
          onSubmitSuccess={(updatedHouse) => {
            setSelectedHouse(updatedHouse);
            setMode("rooms");
          }}
        />
      </section>
    );
  }

  function renderCreateHouseEditor() {
    return (
      <section className="editor-panel editor-panel--embedded">
        <button className="back-link" onClick={loadHouses}>
          Quay lại danh sách khu trọ
        </button>
        <RoomingHouseEditor
          title="Tạo khu trọ mới"
          subtitle="Thêm khu trọ mới vào danh sách quản lý của bạn."
          submitLabel="Gửi duyệt"
          onChange={(createdHouse) => {
            setSelectedHouse(createdHouse);
            setHouses((current) =>
              current.some((house) => house.id === createdHouse.id)
                ? current.map((house) =>
                    house.id === createdHouse.id ? createdHouse : house,
                  )
                : [createdHouse, ...current],
            );
          }}
          onSubmitSuccess={(createdHouse) => {
            setSelectedHouse(createdHouse);
            setHouses((current) =>
              current.some((house) => house.id === createdHouse.id)
                ? current.map((house) =>
                    house.id === createdHouse.id ? createdHouse : house,
                  )
                : [createdHouse, ...current],
            );
            setMode("houses");
          }}
        />
      </section>
    );
  }

  function renderRoomEditor() {
    if (mode !== "create-room" && !selectedRoom) {
      return null;
    }

    return (
      <section className="editor-panel">
        <button
          className="back-link"
          onClick={() => setMode(mode === "create-room" ? "rooms" : "room-detail")}
        >
          {mode === "create-room" ? "Quay lại danh sách phòng" : "Quay lại thông tin phòng"}
        </button>
        <Tabs
          tabs={[
            ["basic", "Thông tin cơ bản"],
            ["images", "Ảnh"],
            ["amenities", "Tiện ích"],
            ["price", "Bảng giá phòng"],
          ]}
          activeTab={roomTab}
          onSelect={(tab) => setRoomTab(tab as RoomTab)}
          disabledTabs={
            mode === "create-room" && !selectedRoom
              ? ["images", "amenities", "price"]
              : []
          }
        />

        {roomTab === "basic" && (
          <div className="form-grid">
            <TextField label="Số phòng" value={roomForm.roomNumber} onChange={(value) => setRoomForm({ ...roomForm, roomNumber: value })} />
            <NumberField label="Tầng" value={roomForm.floor} onChange={(value) => setRoomForm({ ...roomForm, floor: value ?? 1 })} />
            <NumberField label="Diện tích" value={roomForm.areaM2} onChange={(value) => setRoomForm({ ...roomForm, areaM2: value })} />
            <NumberField label="Số người tối đa" value={roomForm.maxOccupants} onChange={(value) => setRoomForm({ ...roomForm, maxOccupants: value ?? 1 })} />
            <TextField label="Mô tả" value={roomForm.description ?? ""} onChange={(value) => setRoomForm({ ...roomForm, description: value })} />
            <SaveRow onSave={saveRoomBasic} />
          </div>
        )}

        {roomTab === "images" && (
          <PropertyImageEditor
            images={roomImages}
            scope="Room"
            onChange={setRoomImages}
            onSave={saveRoomImages}
          />
        )}

        {roomTab === "amenities" && (
          <AmenityEditor
            amenities={roomAmenities}
            selectedIds={roomAmenityIds}
            onChange={setRoomAmenityIds}
            onSave={saveRoomAmenities}
          />
        )}

        {roomTab === "price" && (
          <PriceTierEditor
            priceTiers={priceTiers}
            onChange={setPriceTiers}
            onSave={savePriceTiers}
          />
        )}
      </section>
    );
  }

  return (
    <div className="landlord-dashboard">
      <aside className="dashboard-sidebar">
        <h1>Chủ trọ</h1>
        <button className="sidebar-item active" onClick={loadHouses}>
          Khu trọ của tôi
        </button>
        <button className="sidebar-item" disabled>
          Tổng quan
        </button>
        <button className="sidebar-item" disabled>
          Yêu cầu thuê
        </button>
      </aside>

      <main className="dashboard-main">
        {renderOverview()}
        {message && <p className="dashboard-message">{message}</p>}
        {sectionLoading && <p className="dashboard-message">Đang xử lý...</p>}
        {renderContent()}
      </main>
    </div>
  );
}

function Overview({
  title,
  subtitle,
  stats,
  actions,
}: {
  title: string;
  subtitle: string;
  stats: Array<[string, string | number]>;
  actions: ReactNode;
}) {
  return (
    <section className="overview-band">
      <div>
        <p className="eyebrow">Quản lý</p>
        <h2>{title}</h2>
        <p>{subtitle}</p>
      </div>
      <div className="overview-stats">
        {stats.map(([label, value]) => (
          <div className="stat-item" key={label}>
            <span>{label}</span>
            <strong>{value}</strong>
          </div>
        ))}
      </div>
      <div className="overview-actions">{actions}</div>
    </section>
  );
}

function CardHeader({ title, meta }: { title: string; meta: string }) {
  return (
    <div className="card-header">
      <h3>{title}</h3>
      <span>{meta}</span>
    </div>
  );
}

function StatusRow({
  approvalStatus,
  visibilityStatus,
}: {
  approvalStatus: string;
  visibilityStatus: string;
}) {
  return (
    <div className="status-row">
      <span className={`status-pill ${getStatusToneClass(approvalStatus)}`}>
        {formatStatus(approvalStatus)}
      </span>
      <span className={`status-pill ${getStatusToneClass(visibilityStatus)}`}>
        {formatStatus(visibilityStatus)}
      </span>
    </div>
  );
}

function Tabs({
  tabs,
  activeTab,
  disabledTabs = [],
  onSelect,
}: {
  tabs: Array<[string, string]>;
  activeTab: string;
  disabledTabs?: string[];
  onSelect: (tab: string) => void;
}) {
  return (
    <div className="tabs">
      {tabs.map(([tab, label]) => (
        <button
          className={activeTab === tab ? "active" : ""}
          disabled={disabledTabs.includes(tab)}
          key={tab}
          onClick={() => onSelect(tab)}
        >
          {label}
        </button>
      ))}
    </div>
  );
}

function TextField({
  label,
  value,
  onChange,
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
}) {
  return (
    <label className="field">
      <span>{label}</span>
      <input value={value} onChange={(event) => onChange(event.target.value)} />
    </label>
  );
}

function NumberField({
  label,
  value,
  onChange,
}: {
  label: string;
  value?: number | null;
  onChange: (value: number | null) => void;
}) {
  return (
    <label className="field">
      <span>{label}</span>
      <input
        type="number"
        value={value ?? ""}
        onChange={(event) =>
          onChange(event.target.value === "" ? null : Number(event.target.value))
        }
      />
    </label>
  );
}

function SaveRow({ onSave }: { onSave: () => void }) {
  return (
    <div className="save-row">
      <button className="primary-action" onClick={onSave}>
        Lưu
      </button>
    </div>
  );
}

function AmenityEditor({
  amenities,
  selectedIds,
  onChange,
  onSave,
}: {
  amenities: Amenity[];
  selectedIds: number[];
  onChange: (selectedIds: number[]) => void;
  onSave: () => void;
}) {
  return (
    <div className="stack-panel">
      <div className="amenity-grid">
        {amenities.map((amenity) => (
          <label className="checkbox-field" key={amenity.id}>
            <input
              type="checkbox"
              checked={selectedIds.includes(amenity.id)}
              onChange={(event) =>
                onChange(
                  event.target.checked
                    ? [...selectedIds, amenity.id]
                    : selectedIds.filter((id) => id !== amenity.id),
                )
              }
            />
            {amenity.name}
          </label>
        ))}
      </div>
      <SaveRow onSave={onSave} />
    </div>
  );
}

function PriceTierEditor({
  priceTiers,
  onChange,
  onSave,
}: {
  priceTiers: RoomPriceTierRequest[];
  onChange: (priceTiers: RoomPriceTierRequest[]) => void;
  onSave: () => void;
}) {
  function updateTier(index: number, tier: RoomPriceTierRequest) {
    onChange(priceTiers.map((item, itemIndex) => (itemIndex === index ? tier : item)));
  }

  return (
    <div className="stack-panel">
      {priceTiers.map((tier, index) => (
        <div className="inline-editor" key={index}>
          <NumberField label="Số người" value={tier.occupantCount} onChange={(value) => updateTier(index, { ...tier, occupantCount: value ?? 1 })} />
          <NumberField label="Giá thuê tháng" value={tier.monthlyRent} onChange={(value) => updateTier(index, { ...tier, monthlyRent: value ?? 0 })} />
          <label className="checkbox-field">
            <input
              type="checkbox"
              checked={tier.isActive}
              onChange={(event) => updateTier(index, { ...tier, isActive: event.target.checked })}
            />
            Đang áp dụng
          </label>
          <button onClick={() => onChange(priceTiers.filter((_, itemIndex) => itemIndex !== index))}>
            Xóa
          </button>
        </div>
      ))}
      <div className="save-row">
        <button
          onClick={() =>
            onChange([
              ...priceTiers,
              { occupantCount: priceTiers.length + 1, monthlyRent: 0, isActive: true },
            ])
          }
        >
          Thêm giá
        </button>
        <button className="primary-action" onClick={onSave}>
          Lưu
        </button>
      </div>
    </div>
  );
}

function toImageRequests(
  images: Array<{
    id: string;
    objectKey: string;
    imageUrl?: string;
    caption?: string | null;
    isCover: boolean;
    sortOrder: number;
  }>,
) {
  return images.map((image) => ({
    id: image.id,
    objectKey: image.objectKey,
    imageUrl: image.imageUrl,
    caption: image.caption,
    isCover: image.isCover,
    sortOrder: image.sortOrder,
  }));
}

function cleanImages(images: PropertyImageRequest[]) {
  return images
    .filter((image) => image.objectKey.trim().length > 0)
    .map((image, index) => ({
      id: image.id,
      objectKey: image.objectKey.trim(),
      caption: image.caption,
      isCover: image.isCover,
      sortOrder: image.sortOrder || index + 1,
    }));
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  }).format(new Date(value));
}

function formatStatus(status: string) {
  const statusLabels: Record<string, string> = {
    Approved: "Đã duyệt",
    Pending: "Chờ duyệt",
    Rejected: "Bị từ chối",
    Draft: "Bản nháp",
    Submitted: "Đã gửi duyệt",
    Public: "Đang hiển thị",
    Visible: "Đang hiển thị",
    Hidden: "Đang ẩn",
    Private: "Đang ẩn",
    Available: "Còn trống",
    Occupied: "Đang thuê",
    Maintenance: "Bảo trì",
  };

  return statusLabels[status] ?? status;
}

function getStatusToneClass(status: string) {
  const toneMap: Record<string, string> = {
    Approved: "status-pill--success",
    Available: "status-pill--success",
    Visible: "status-pill--info",
    Public: "status-pill--info",
    Pending: "status-pill--warning",
    Submitted: "status-pill--warning",
    Rejected: "status-pill--danger",
    Occupied: "status-pill--danger",
    Draft: "status-pill--neutral",
    Hidden: "status-pill--neutral",
    Private: "status-pill--neutral",
    Reserved: "status-pill--reserved",
    Maintenance: "status-pill--maintenance",
  };

  return toneMap[status] ?? "status-pill--neutral";
}

function getCreateHouseBlockedMessage(status: string) {
  if (status === "Draft") {
    return "Bạn đang có bản nháp khu trọ. Vui lòng hoàn thành bản nháp và gửi duyệt trước khi thêm khu trọ mới.";
  }

  if (status === "Pending") {
    return "Bạn đang có khu trọ chờ duyệt. Vui lòng chờ kết quả xét duyệt trước khi thêm khu trọ mới.";
  }

  if (status === "Rejected") {
    return "Bạn đang có khu trọ bị từ chối. Vui lòng chỉnh sửa và gửi duyệt lại trước khi thêm khu trọ mới.";
  }

  return "Bạn cần xử lý hồ sơ khu trọ hiện tại trước khi thêm khu trọ mới.";
}
