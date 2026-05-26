import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getCurrentUser } from "../features/users/api";
import type { CurrentUser } from "../features/users/types";
import "./HomePage.css";

export default function HomePage() {
  const [user, setUser] = useState<CurrentUser | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function loadCurrentUser() {
      try {
        const data = await getCurrentUser();
        setUser(data);
      } catch {
        setUser(null);
      } finally {
        setLoading(false);
      }
    }

    loadCurrentUser();
  }, []);

  const isLandlord = user?.roles.includes("Landlord") === true;

  function logout() {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    setUser(null);
  }

  return (
    <div className="home-page">
      <header className="home-header">
        <Link className="home-brand" to="/">
          <strong>Smart Rental Platform</strong>
          <span>Tìm trọ và quản lý khu trọ</span>
        </Link>

        <nav className="home-nav">
          <Link to="/">Trang chủ</Link>
          {!loading && user && isLandlord && (
            <Link className="home-nav__primary" to="/landlord/rooming-houses">
              Kênh chủ trọ
            </Link>
          )}
          {!loading && user && !isLandlord && (
            <Link className="home-nav__primary" to="/landlord/rooming-houses/create">
              Đăng ký trở thành chủ trọ
            </Link>
          )}
          {!loading && user ? (
            <button onClick={logout}>Đăng xuất</button>
          ) : (
            <Link className="home-nav__primary" to="/login">
              Đăng nhập
            </Link>
          )}
        </nav>
      </header>

      <main className="home-main">
        <section className="home-hero">
          <div>
            <h1>Không gian thuê trọ rõ ràng hơn cho người thuê và chủ trọ.</h1>
            <p>
              Quản lý khu trọ, phòng, tiện ích và yêu cầu thuê trong cùng một
              hệ thống. Chủ trọ có thể cập nhật thông tin, còn người thuê dễ
              dàng theo dõi lựa chọn phù hợp.
            </p>
          </div>

          <aside className="home-status">
            <span>Trạng thái tài khoản</span>
            {loading && <strong>Đang tải thông tin...</strong>}
            {!loading && user && <strong>Xin chào {user.fullName || user.email}</strong>}
            {!loading && !user && <strong>Bạn chưa đăng nhập</strong>}
          </aside>
        </section>

        <section className="home-panel">
          <h2>Trang chủ</h2>
          <p>
            Khu vực này sẽ hiển thị danh sách phòng trọ, bộ lọc tìm kiếm và các
            đề xuất phù hợp trong các giai đoạn tiếp theo.
          </p>
        </section>
      </main>
    </div>
  );
}
