import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { getApiErrorMessage } from "../../shared/api/apiError";
import { getMyRoomingHouseOnboarding } from "./api";
import RoomingHouseEditor from "./components/RoomingHouseEditor";
import type {
  RoomingHouseDetail,
  RoomingHouseOnboarding,
} from "./types";
import "./CreateRoomingHousePage.css";

export default function CreateRoomingHousePage() {
  const [onboarding, setOnboarding] =
    useState<RoomingHouseOnboarding | null>(null);
  const [roomingHouse, setRoomingHouse] =
    useState<RoomingHouseDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState("");

  useEffect(() => {
    async function loadOnboarding() {
      try {
        const data = await getMyRoomingHouseOnboarding();
        setOnboarding(data);

        if (shouldContinueExistingDraft(data)) {
          setRoomingHouse(data.roomingHouse ?? null);
        }
      } catch (error) {
        setMessage(
          getApiErrorMessage(error, "Không thể tải trạng thái đăng ký chủ trọ."),
        );
      } finally {
        setLoading(false);
      }
    }

    loadOnboarding();
  }, []);

  const pageCopy = useMemo(() => {
    if (onboarding?.canEnterLandlordDashboard) {
      return {
        title: "Tạo khu trọ mới",
        subtitle: "Bổ sung thông tin khu trọ để gửi quản trị viên xét duyệt.",
        submitLabel: "Gửi duyệt khu trọ",
      };
    }

    if (onboarding?.status === "Rejected") {
      return {
        title: "Bổ sung hồ sơ chủ trọ",
        subtitle: "Cập nhật khu trọ theo lý do từ chối và gửi duyệt lại.",
        submitLabel: "Gửi duyệt lại",
      };
    }

    if (onboarding?.status === "Draft") {
      return {
        title: "Tiếp tục đăng ký chủ trọ",
        subtitle: "Hoàn tất thông tin khu trọ đầu tiên để gửi duyệt hồ sơ chủ trọ.",
        submitLabel: "Gửi duyệt hồ sơ",
      };
    }

    return {
      title: "Đăng ký trở thành chủ trọ",
      subtitle: "Hoàn tất thông tin khu trọ đầu tiên để gửi duyệt hồ sơ chủ trọ.",
      submitLabel: "Gửi duyệt hồ sơ",
    };
  }, [onboarding]);

  if (loading) {
    return <main className="create-rooming-house-page">Đang tải...</main>;
  }

  if (message) {
    return (
      <main className="create-rooming-house-page">
        <section className="create-rooming-house-page__state">
          <h1>Không thể mở trang tạo khu trọ</h1>
          <p>{message}</p>
        </section>
      </main>
    );
  }

  if (onboarding && !canEditOrCreate(onboarding)) {
    return (
      <main className="create-rooming-house-page">
        <section className="create-rooming-house-page__state">
          <h1>{getLockedTitle(onboarding.status)}</h1>
          <p>{getLockedDescription(onboarding.status)}</p>
          {onboarding.roomingHouse && (
            <div className="create-rooming-house-page__summary">
              <strong>{onboarding.roomingHouse.name}</strong>
              <span>{onboarding.roomingHouse.addressDisplay}</span>
              {onboarding.roomingHouse.rejectedReason && (
                <span>Lý do từ chối: {onboarding.roomingHouse.rejectedReason}</span>
              )}
            </div>
          )}
          {onboarding.canEnterLandlordDashboard && (
            <Link className="create-rooming-house-page__primary" to="/landlord/rooming-houses">
              Vào kênh chủ trọ
            </Link>
          )}
        </section>
      </main>
    );
  }

  return (
    <main className="create-rooming-house-page">
      <RoomingHouseEditor
        title={pageCopy.title}
        subtitle={pageCopy.subtitle}
        initialRoomingHouse={roomingHouse}
        submitLabel={pageCopy.submitLabel}
        onChange={setRoomingHouse}
        onSubmitSuccess={setRoomingHouse}
      />
    </main>
  );
}

function shouldContinueExistingDraft(onboarding: RoomingHouseOnboarding) {
  if (onboarding.canEnterLandlordDashboard && onboarding.canCreateDraft) {
    return false;
  }

  return onboarding.canEdit && Boolean(onboarding.roomingHouse);
}

function canEditOrCreate(onboarding: RoomingHouseOnboarding) {
  return onboarding.canCreateDraft || onboarding.canEdit;
}

function getLockedTitle(status: string) {
  if (status === "Pending") {
    return "Hồ sơ đang chờ duyệt";
  }

  if (status === "Approved") {
    return "Bạn đã trở thành chủ trọ";
  }

  return "Chưa thể tạo khu trọ mới";
}

function getLockedDescription(status: string) {
  if (status === "Pending") {
    return "Khu trọ của bạn đã được gửi cho quản trị viên xét duyệt. Vui lòng chờ kết quả.";
  }

  if (status === "Approved") {
    return "Khu trọ đầu tiên của bạn đã được duyệt. Bạn có thể vào kênh chủ trọ để quản lý hoặc tạo thêm khu trọ.";
  }

  return "Hiện tại bạn đang có một hồ sơ khu trọ cần xử lý trước khi tạo mới.";
}
