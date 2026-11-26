import { useState } from "react";
import { Link } from "react-router-dom";
import "../styles/pages/forgot-password.css";
import { useI18n } from "../lib/i18n";
import { useTheme } from "../lib/theme";
import { usePageMeta } from "../lib/hooks";

const EMAIL_RE = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export default function ForgotPassword() {
  const { t, lang } = useI18n();
  const { theme } = useTheme();
  usePageMeta(t("forgot.title", "Forgot Password"));

  const [email, setEmail] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [shake, setShake] = useState(false);
  const [sent, setSent] = useState(false);
  const [resendState, setResendState] = useState("idle");

  const logoSrc = `/images/logo-${lang === "fa" ? "fa" : "en"}-${theme === "light" ? "light" : "dark"}.svg`;

  const onSubmit = (e) => {
    e.preventDefault();

    const err = !email.trim()
      ? t("forgot.error_email_required", "Email is required")
      : !EMAIL_RE.test(email)
        ? t("forgot.error_email_invalid", "Please enter a valid email")
        : "";
    if (err) {
      setError(err);
      setShake(false);
      requestAnimationFrame(() => setShake(true));
      return;
    }

    setLoading(true);
    setTimeout(() => {
      setLoading(false);
      setSent(true);
    }, 1500);
  };

  const onResend = () => {
    if (resendState === "sent") return;
    setResendState("sent");
    setTimeout(() => setResendState("idle"), 3000);
  };

  return (
    <section className="forgot-section">
      <div className="forgot-bg-gradient" aria-hidden="true"></div>

      <div className={`forgot-card${shake ? " shake" : ""}`} onAnimationEnd={() => setShake(false)}>
        {!sent ? (
          <div className="forgot-step">
            <Link to="/Login" className="forgot-back-link">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="18" height="18">
                <path d="m15 18-6-6 6-6" />
              </svg>
              <span>{t("forgot.back_to_login", "Back to login")}</span>
            </Link>

            <div className="forgot-logo">
              <img src={logoSrc} alt="Logo" className="forgot-logo-img" />
            </div>

            <h1 className="forgot-title">{t("forgot.heading", "Forgot Password?")}</h1>
            <p className="forgot-subtitle">{t("forgot.subtitle", "No worries! Enter your email and we'll send you a reset link.")}</p>

            <form className="forgot-form" noValidate onSubmit={onSubmit}>
              <div className={`forgot-form-group${error ? " has-error" : ""}`}>
                <label className="forgot-form-label" htmlFor="forgotEmail">{t("forgot.email_label", "EMAIL")}</label>
                <div className="forgot-input-wrap">
                  <svg className="forgot-input-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="18" height="18">
                    <rect width="20" height="16" x="2" y="4" rx="2" />
                    <path d="m22 7-8.97 5.7a1.94 1.94 0 0 1-2.06 0L2 7" />
                  </svg>
                  <input
                    type="email"
                    id="forgotEmail"
                    name="email"
                    className="forgot-input"
                    placeholder={t("forgot.email_placeholder", "you@email.com")}
                    autoComplete="email"
                    value={email}
                    onChange={(e) => {
                      setEmail(e.target.value);
                      if (error) setError("");
                    }}
                  />
                </div>
                <span className="forgot-error" aria-live="polite">{error}</span>
              </div>

              <button type="submit" className={`forgot-submit-btn${loading ? " loading" : ""}`} disabled={loading}>
                <span className="forgot-submit-text">{t("forgot.send_link", "Send Reset Link")}</span>
                <span className="forgot-submit-spinner" aria-hidden="true"></span>
                <span className="forgot-submit-arrow">&rarr;</span>
              </button>
            </form>
          </div>
        ) : (
          <div className="forgot-step">
            <div className="forgot-success-icon-wrap">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="36" height="36">
                <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14" />
                <path d="m9 11 3 3L22 4" />
              </svg>
            </div>

            <h2 className="forgot-title">{t("forgot.check_email", "Check Your Email")}</h2>
            <p className="forgot-subtitle">
              {t("forgot.email_sent", "We've sent a password reset link to your email address. Please check your inbox and follow the instructions.")}
            </p>

            <div className="forgot-email-badge">{email}</div>

            <p className="forgot-resend-text">
              <span>{t("forgot.didnt_receive", "Didn't receive the email?")}</span>
              <button type="button" className="forgot-resend-btn" onClick={onResend} disabled={resendState === "sent"}>
                {resendState === "sent" ? t("forgot.sent", "Sent!") : t("forgot.resend", "Resend")}
              </button>
            </p>

            <Link to="/Login" className="forgot-back-btn">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="16" height="16">
                <path d="m15 18-6-6 6-6" />
              </svg>
              <span>{t("forgot.back_to_login", "Back to login")}</span>
            </Link>
          </div>
        )}
      </div>
    </section>
  );
}
