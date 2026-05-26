import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { getApiErrorMessage } from "../../shared/api/apiError";
import { getAmenities } from "../rooming-houses/api";
import PropertyImageEditor from "../rooming-houses/components/PropertyImageEditor";
import type { Amenity, PropertyImageRequest } from "../rooming-houses/types";
import {
  createRoom,
  submitRoom,
  updateRoom,
  updateRoomAmenities,
  updateRoomImages,
  updateRoomPriceTiers,
} from "./api";
import type { CreateRoomRequest, Room, RoomPriceTierRequest } from "./types";
import "./CreateRoomPage.css";

type RoomTab = "basic" | "images" | "amenities" | "price";

const emptyRoomForm: CreateRoomRequest = {
  roomNumber: "",
  floor: 1,
  areaM2: null,
  maxOccupants: 1,
  description: "",
};

export default function CreateRoomPage() {
  const { roomingHouseId } = useParams();
  const [room, setRoom] = useState<Room | null>(null);
  const [activeTab, setActiveTab] = useState<RoomTab>("basic");
  const [amenities, setAmenities] = useState<Amenity[]>([]);
  const [roomForm, setRoomForm] = useState<CreateRoomRequest>(emptyRoomForm);
  const [roomImages, setRoomImages] = useState<PropertyImageRequest[]>([]);
  const [roomAmenityIds, setRoomAmenityIds] = useState<number[]>([]);
  const [priceTiers, setPriceTiers] = useState<RoomPriceTierRequest[]>([
    { occupantCount: 1, monthlyRent: 0, isActive: true },
  ]);
  const [message, setMessage] = useState("");
  const [saving, setSaving] = useState(false);

  const hasRoom = Boolean(room?.id);

  useEffect(() => {
    async function loadAmenities() {
      try {
        setAmenities(await getAmenities("Room"));
      } catch (error) {
        setMessage(
          getApiErrorMessage(error, "Không thể tải danh mục tiện ích phòng."),
        );
      }
    }

    loadAmenities();
  }, []);

  async function saveBasicInfo() {
    if (!roomingHouseId) {
      setMessage("Thiếu mã khu trọ trên đường dẫn.");
      return;
    }

    await saveChange(async () => {
      const saved = room
        ? await updateRoom(room.id, roomForm)
        : await createRoom(roomingHouseId, roomForm);

      if (!room) {
        setActiveTab("images");
      }

      return saved;
    }, "Đã lưu thông tin cơ bản của phòng.");
  }

  async function saveRoomImages() {
    if (!room) {
      setMessage("Vui lòng lưu thông tin cơ bản trước khi thêm ảnh.");
      return;
    }

    await saveChange(
      () => updateRoomImages(room.id, cleanImages(roomImages)),
      "Đã lưu ảnh phòng.",
    );
  }

  async function saveRoomAmenities() {
    if (!room) {
      setMessage("Vui lòng lưu thông tin cơ bản trước khi thêm tiện ích.");
      return;
    }

    await saveChange(
      () => updateRoomAmenities(room.id, roomAmenityIds),
      "Đã lưu tiện ích phòng.",
    );
  }

  async function savePriceTiers() {
    if (!room) {
      setMessage("Vui lòng lưu thông tin cơ bản trước khi thêm bảng giá.");
      return;
    }

    await saveChange(
      () => updateRoomPriceTiers(room.id, priceTiers),
      "Đã lưu bảng giá phòng.",
    );
  }

  async function publishRoom() {
    if (!room) {
      setMessage("Vui lòng lưu thông tin phòng trước khi hiển thị.");
      return;
    }

    await saveChange(
      () => submitRoom(room.id),
      "Phòng đã được hiển thị và chuyển sang trạng thái còn trống.",
    );
  }

  async function saveChange(
    request: () => Promise<Room>,
    successMessage: string,
  ) {
    setSaving(true);
    setMessage("");

    try {
      const saved = await request();
      setRoom(saved);
      hydrateRoom(saved);
      setMessage(successMessage);
    } catch (error) {
      setMessage(getApiErrorMessage(error, "Không thể lưu thông tin phòng."));
    } finally {
      setSaving(false);
    }
  }

  function hydrateRoom(saved: Room) {
    setRoomForm({
      roomNumber: saved.roomNumber,
      floor: saved.floor,
      areaM2: saved.areaM2 ?? null,
      maxOccupants: saved.maxOccupants,
      description: saved.description ?? "",
    });
    setRoomImages(toImageRequests(saved.images));
    setRoomAmenityIds(saved.amenities.map((amenity) => amenity.id));
    setPriceTiers(
      saved.priceTiers.length > 0
        ? saved.priceTiers.map((tier) => ({
            occupantCount: tier.occupantCount,
            monthlyRent: tier.monthlyRent,
            isActive: tier.isActive,
          }))
        : [{ occupantCount: 1, monthlyRent: 0, isActive: true }],
    );
  }

  return (
    <main className="create-room-page">
      <header className="create-room-page__header">
        <div>
          <p className="eyebrow">Quản lý phòng</p>
          <h1>Tạo phòng mới</h1>
          <p>
            Lưu thông tin cơ bản trước, sau đó bổ sung ảnh, tiện ích và bảng
            giá cho phòng.
          </p>
        </div>
        <Link to="/landlord/rooming-houses">Quay lại kênh chủ trọ</Link>
      </header>

      {room && (
        <div className="create-room-page__status-row">
          <p className="create-room-page__status">
            Trạng thái phòng: {formatStatus(room.status)}
          </p>
          {room.status === "Hidden" && (
            <button className="primary-action" onClick={publishRoom}>
              Hiển thị phòng
            </button>
          )}
        </div>
      )}
      {message && <p className="create-room-page__message">{message}</p>}
      {saving && <p className="create-room-page__message">Đang xử lý...</p>}

      <section className="create-room-page__panel">
        <div className="tabs">
          <TabButton activeTab={activeTab} tab="basic" onSelect={setActiveTab}>
            Thông tin cơ bản
          </TabButton>
          <TabButton activeTab={activeTab} tab="images" disabled={!hasRoom} onSelect={setActiveTab}>
            Ảnh
          </TabButton>
          <TabButton activeTab={activeTab} tab="amenities" disabled={!hasRoom} onSelect={setActiveTab}>
            Tiện ích
          </TabButton>
          <TabButton activeTab={activeTab} tab="price" disabled={!hasRoom} onSelect={setActiveTab}>
            Bảng giá phòng
          </TabButton>
        </div>

        {activeTab === "basic" && (
          <div className="form-grid">
            <TextField label="Số phòng" value={roomForm.roomNumber} onChange={(value) => setRoomForm({ ...roomForm, roomNumber: value })} />
            <NumberField label="Tầng" value={roomForm.floor} onChange={(value) => setRoomForm({ ...roomForm, floor: value ?? 1 })} />
            <NumberField label="Diện tích" value={roomForm.areaM2} onChange={(value) => setRoomForm({ ...roomForm, areaM2: value })} />
            <NumberField label="Số người tối đa" value={roomForm.maxOccupants} onChange={(value) => setRoomForm({ ...roomForm, maxOccupants: value ?? 1 })} />
            <TextField label="Mô tả" value={roomForm.description ?? ""} onChange={(value) => setRoomForm({ ...roomForm, description: value })} />
            <div className="save-row">
              <button className="primary-action" onClick={saveBasicInfo}>
                {hasRoom ? "Lưu" : "Lưu và tiếp tục"}
              </button>
            </div>
          </div>
        )}

        {activeTab === "images" && (
          <PropertyImageEditor
            images={roomImages}
            scope="Room"
            onChange={setRoomImages}
            onSave={saveRoomImages}
          />
        )}

        {activeTab === "amenities" && (
          <AmenityEditor
            amenities={amenities}
            selectedIds={roomAmenityIds}
            onChange={setRoomAmenityIds}
            onSave={saveRoomAmenities}
          />
        )}

        {activeTab === "price" && (
          <PriceTierEditor
            priceTiers={priceTiers}
            onChange={setPriceTiers}
            onSave={savePriceTiers}
          />
        )}
      </section>
    </main>
  );
}

function TabButton({
  activeTab,
  tab,
  disabled,
  children,
  onSelect,
}: {
  activeTab: RoomTab;
  tab: RoomTab;
  disabled?: boolean;
  children: string;
  onSelect: (tab: RoomTab) => void;
}) {
  return (
    <button
      className={activeTab === tab ? "active" : ""}
      disabled={disabled}
      onClick={() => onSelect(tab)}
    >
      {children}
    </button>
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
      <div className="save-row">
        <button className="primary-action" onClick={onSave}>
          Lưu
        </button>
      </div>
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

function formatStatus(status: string) {
  const statusLabels: Record<string, string> = {
    Available: "Còn trống",
    Occupied: "Đang thuê",
    Hidden: "Đang ẩn",
    Maintenance: "Bảo trì",
  };

  return statusLabels[status] ?? status;
}
