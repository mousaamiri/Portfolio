import { useState } from "react";
import { Link } from "react-router-dom";
import "../styles/pages/login.css";
import { useI18n } from "../lib/i18n";
import { useTheme } from "../lib/theme";
import { usePageMeta } from "../lib/hooks";

const EMAIL_RE = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export default function Login() {
  const { t, lang } = useI18n();
  const { theme } = useTheme();
  usePageMeta(t("login.title", "Login"));

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [remember, setRemember] = useState(false);
  const [errors, setErrors] = useState({ email: "", password: "" });
  const [feedback, setFeedback] = useState({ type: "", message: "" });
  const [loading, setLoading] = useState(false);
  const [shake, setShake] = useState(false);

  const logoSrc = `/images/logo-${lang === "fa" ? "fa" : "en"}-${theme === "light" ? "light" : "dark"}.svg`;

  const onSubmit = (e) => {
    e.preventDefault();
    setFeedback({ type: "", message: "" });

    const emailErr = !email.trim()
      ? t("login.error_email_required", "Email is required")
      : !EMAIL_RE.test(email)
        ? t("login.error_email_invalid", "Please enter a valid email")
        : "";
    const passwordErr = !password
      ? t("login.error_password_required", "Password is required")
      : password.length < 6
        ? t("login.error_password_short", "Password must be at least 6 characters")
        : "";

    setErrors({ email: emailErr, password: passwordErr });

    if (emailErr || passwordErr) {
      setShake(false);
      requestAnimationFrame(() => setShake(true));
      return;
    }

    setLoading(true);
    setTimeout(() => {
      setLoading(false);
      setFeedback({ type: "success", message: t("login.success", "Login successful! Redirecting...") });
    }, 1800);
  };

  return (
    <section className="login-section">
      <div className="login-bg-gradient" aria-hidden="true"></div>

      <div className={`login-card${shake ? " shake" : ""}`} onAnimationEnd={() => setShake(false)}>
        <div className="login-logo">
          <img src={logoSrc} alt="Logo" className="login-logo-img" />
        </div>

        <h1 className="login-title">{t("login.heading", "Welcome Back")}</h1>
        <p className="login-subtitle">{t("login.subtitle", "Sign in to your account to continue")}</p>

        <form className="login-form" noValidate onSubmit={onSubmit}>
          <div className={`login-form-group${errors.email ? " has-error" : ""}`}>
            <label className="login-form-label" htmlFor="loginEmail">{t("login.email_label", "EMAIL")}</label>
            <div className="login-input-wrap">
              <svg className="login-input-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="18" height="18">
                <rect width="20" height="16" x="2" y="4" rx="2" />
                <path d="m22 7-8.97 5.7a1.94 1.94 0 0 1-2.06 0L2 7" />
              </svg>
              <input
                type="email"
                id="loginEmail"
                name="email"
                className="login-input"
                placeholder={t("login.email_placeholder", "you@email.com")}
                autoComplete="email"
                value={email}
                onChange={(e) => {
                  setEmail(e.target.value);
                  if (errors.email) setErrors((s) => ({ ...s, email: "" }));
                }}
              />
            </div>
            <span className="login-error" aria-live="polite">{errors.email}</span>
          </div>

          <div className={`login-form-group${errors.password ? " has-error" : ""}`}>
            <label className="login-form-label" htmlFor="loginPassword">{t("login.password_label", "PASSWORD")}</label>
            <div className="login-input-wrap">
              <svg className="login-input-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="18" height="18">
                <rect width="18" height="11" x="3" y="11" rx="2" ry="2" />
                <path d="M7 11V7a5 5 0 0 1 10 0v4" />
              </svg>
              <input
                type={showPassword ? "text" : "password"}
                id="loginPassword"
                name="password"
                className="login-input login-input--password"
                placeholder={t("login.password_placeholder", "Enter your password")}
                autoComplete="current-password"
                value={password}
                onChange={(e) => {
                  setPassword(e.target.value);
                  if (errors.password) setErrors((s) => ({ ...s, password: "" }));
                }}
              />
              <button
                type="button"
                className={`login-toggle-password${showPassword ? " active" : ""}`}
                aria-label={showPassword ? "Hide password" : "Show password"}
                onClick={() => setShowPassword((v) => !v)}
              >
                {showPassword ? (
                  <svg className="icon-eye-off" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="18" height="18">
                    <path d="M10.733 5.076a10.744 10.744 0 0 1 11.205 6.575 1 1 0 0 1 0 .696 10.747 10.747 0 0 1-1.444 2.49" />
                    <path d="M14.084 14.158a3 3 0 0 1-4.242-4.242" />
                    <path d="M17.479 17.499a10.75 10.75 0 0 1-15.417-5.151 1 1 0 0 1 0-.696 10.75 10.75 0 0 1 4.446-5.143" />
                    <path d="m2 2 20 20" />
                  </svg>
                ) : (
                  <svg className="icon-eye" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="18" height="18">
                    <path d="M2.062 12.348a1 1 0 0 1 0-.696 10.75 10.75 0 0 1 19.876 0 1 1 0 0 1 0 .696 10.75 10.75 0 0 1-19.876 0" />
                    <circle cx="12" cy="12" r="3" />
                  </svg>
                )}
              </button>
            </div>
            <span className="login-error" aria-live="polite">{errors.password}</span>
          </div>

          <div className="login-options">
            <label className="login-checkbox-label">
              <input type="checkbox" className="login-checkbox" checked={remember} onChange={(e) => setRemember(e.target.checked)} />
              <span className="login-checkbox-custom"></span>
              <span>{t("login.remember_me", "Remember me")}</span>
            </label>
            <Link to="/ForgotPassword" className="login-forgot-link">{t("login.forgot_password", "Forgot password?")}</Link>
          </div>

          <button type="submit" className={`login-submit-btn${loading ? " loading" : ""}`} disabled={loading}>
            <span className="login-submit-text">{t("login.sign_in", "Sign In")}</span>
            <span className="login-submit-spinner" aria-hidden="true"></span>
            <span className="login-submit-arrow">&rarr;</span>
          </button>

          <div className={`login-form-feedback ${feedback.type}`} aria-live="polite">{feedback.message}</div>
        </form>
      </div>
    </section>
  );
}
