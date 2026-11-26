import { useEffect, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import "../styles/pages/login.css";
import { useTheme } from "../lib/theme";
import { usePageMeta } from "../lib/hooks";
import { api, isAuthenticated, setToken } from "../lib/api";

export default function AdminLogin() {
  const { theme } = useTheme();
  const navigate = useNavigate();
  const location = useLocation();
  usePageMeta("Admin Login");

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [remember, setRemember] = useState(false);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [shake, setShake] = useState(false);

  const redirectTo = location.state?.from || "/Admin";
  const logoSrc = `/images/logo-en-${theme === "light" ? "light" : "dark"}.svg`;

  useEffect(() => {
    if (isAuthenticated()) navigate("/Admin", { replace: true });
  }, [navigate]);

  useEffect(() => {
    const prevDir = document.documentElement.getAttribute("dir");
    document.documentElement.setAttribute("dir", "ltr");
    return () => {
      if (prevDir) document.documentElement.setAttribute("dir", prevDir);
    };
  }, []);

  const onSubmit = async (e) => {
    e.preventDefault();
    setError("");

    if (!username.trim() || !password) {
      setError("Enter your username and password.");
      setShake(false);
      requestAnimationFrame(() => setShake(true));
      return;
    }

    setLoading(true);
    try {
      const result = await api.adminLogin(username.trim(), password);
      if (!result?.token) {
        setError("Invalid username or password.");
        setShake(false);
        requestAnimationFrame(() => setShake(true));
        return;
      }
      setToken(result.token, result.expiresAt);
      navigate(redirectTo, { replace: true });
    } catch {
      setError("Could not reach the server. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <section className="login-section">
      <div className="login-bg-gradient" aria-hidden="true"></div>

      <div className={`login-card${shake ? " shake" : ""}`} onAnimationEnd={() => setShake(false)}>
        <div className="login-logo">
          <img src={logoSrc} alt="Logo" className="login-logo-img" />
        </div>

        <h1 className="login-title">Admin Panel</h1>
        <p className="login-subtitle">Sign in to manage your portfolio</p>

        <form className="login-form" noValidate onSubmit={onSubmit}>
          <div className="login-form-group">
            <label className="login-form-label" htmlFor="Username">USERNAME</label>
            <div className="login-input-wrap">
              <svg className="login-input-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="18" height="18">
                <path d="M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2" />
                <circle cx="12" cy="7" r="4" />
              </svg>
              <input
                id="Username"
                className="login-input"
                placeholder="Enter username"
                autoComplete="username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
              />
            </div>
          </div>

          <div className="login-form-group">
            <label className="login-form-label" htmlFor="Password">PASSWORD</label>
            <div className="login-input-wrap">
              <svg className="login-input-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="18" height="18">
                <rect width="18" height="11" x="3" y="11" rx="2" ry="2" />
                <path d="M7 11V7a5 5 0 0 1 10 0v4" />
              </svg>
              <input
                id="Password"
                type={showPassword ? "text" : "password"}
                className="login-input login-input--password"
                placeholder="Enter password"
                autoComplete="current-password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
              <button
                type="button"
                className={`login-toggle-password${showPassword ? " active" : ""}`}
                aria-label={showPassword ? "Hide password" : "Show password"}
                onClick={() => setShowPassword((v) => !v)}
              >
                <svg className="icon-eye" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="18" height="18">
                  <path d="M2.062 12.348a1 1 0 0 1 0-.696 10.75 10.75 0 0 1 19.876 0 1 1 0 0 1 0 .696 10.75 10.75 0 0 1-19.876 0" />
                  <circle cx="12" cy="12" r="3" />
                </svg>
              </button>
            </div>
          </div>

          <div className="login-options">
            <label className="login-checkbox-label">
              <input type="checkbox" className="login-checkbox" checked={remember} onChange={(e) => setRemember(e.target.checked)} />
              <span className="login-checkbox-custom"></span>
              <span>Remember me</span>
            </label>
            <Link to="/Admin/ForgotPassword" className="login-forgot-link">Forgot password?</Link>
          </div>

          <button type="submit" className={`login-submit-btn${loading ? " loading" : ""}`} disabled={loading}>
            <span className="login-submit-text">Sign In</span>
            <span className="login-submit-spinner" aria-hidden="true"></span>
            <span className="login-submit-arrow">&rarr;</span>
          </button>

          {error && <div className="login-form-feedback error" aria-live="polite">{error}</div>}
        </form>
      </div>
    </section>
  );
}
