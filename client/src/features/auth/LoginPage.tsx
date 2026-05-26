import { useState } from "react";
import { Link } from "react-router-dom";
import { getApiErrorMessage } from "../../shared/api/apiError";
import { login } from "./api";
import "./LoginPage.css";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [message, setMessage] = useState("");

  async function handleLogin() {
    try {
      const result = await login({ email, password });
      localStorage.setItem("accessToken", result.accessToken);
      localStorage.setItem("refreshToken", result.refreshToken);
      window.location.href = "/";
    } catch (error) {
      setMessage(getApiErrorMessage(error, "Đăng nhập thất bại."));
    }
  }

  return (
    <main className="login-page">
      <section className="login-panel">
        <div className="login-panel__intro">
          <Link to="/">Smart Rental Platform</Link>
          <h1>Đăng nhập</h1>
          <p>Truy cập tài khoản để quản lý phòng trọ, khu trọ và các yêu cầu thuê.</p>
        </div>

        <div className="login-panel__form">
          <label>
            <span>Email</span>
            <input
              autoComplete="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
            />
          </label>

          <label>
            <span>Mật khẩu</span>
            <input
              autoComplete="current-password"
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
            />
          </label>

          <button onClick={handleLogin}>Đăng nhập</button>

          {message && <p className="login-panel__message">{message}</p>}
        </div>
      </section>
    </main>
  );
}
