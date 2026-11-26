import { Link } from "react-router-dom";
import { useI18n } from "../lib/i18n";
import { usePageMeta } from "../lib/hooks";

// Bilingual error / not-found page (port of Views/Error.cshtml). Kept dependency-light
// so it renders even if the API is unavailable.
const COPY = {
  en: {
    title: "Something went wrong",
    lead: "There was a problem processing your request.",
    hint: "Please try again. If it keeps happening, check back later.",
    retry: "Try again",
    home: "Home",
  },
  fa: {
    title: "یک خطای غیرمنتظره رخ داد",
    lead: "مشکلی در پردازش درخواست شما پیش آمد.",
    hint: "لطفاً دوباره تلاش کنید. اگر مشکل ادامه داشت، بعداً سر بزنید.",
    retry: "تلاش مجدد",
    home: "خانه",
  },
  ar: {
    title: "حدث خطأ غير متوقع",
    lead: "حدثت مشكلة أثناء معالجة طلبك.",
    hint: "يُرجى المحاولة مجددًا. إذا استمرت المشكلة، عُد لاحقًا.",
    retry: "إعادة المحاولة",
    home: "الرئيسية",
  },
};

export default function ErrorPage({ status = 404 }) {
  const { lang } = useI18n();
  const copy = COPY[lang] || COPY.en;
  usePageMeta(copy.title);

  return (
    <section
      style={{
        minHeight: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        padding: "24px",
        background: "var(--bg, #0b0b0f)",
        color: "var(--text, #f4f4f7)",
      }}
    >
      <main
        role="alert"
        style={{
          width: "100%",
          maxWidth: "480px",
          background: "var(--panel, #14141b)",
          border: "1px solid var(--border, rgba(255,255,255,.08))",
          borderRadius: "20px",
          padding: "40px 32px",
          textAlign: "center",
          boxShadow: "0 24px 60px rgba(0,0,0,.28)",
        }}
      >
        <p style={{ fontSize: "13px", fontWeight: 700, letterSpacing: ".18em", color: "var(--muted, #a1a1b3)", margin: "0 0 20px" }}>
          {status}
        </p>
        <div
          aria-hidden="true"
          style={{
            width: "64px",
            height: "64px",
            margin: "0 auto 24px",
            borderRadius: "50%",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            background: "var(--accent-soft, rgba(245,179,1,.25))",
            color: "var(--accent, #f5b301)",
          }}
        >
          <svg viewBox="0 0 24 24" width="32" height="32" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round">
            <circle cx="12" cy="12" r="10" />
            <line x1="12" y1="8" x2="12" y2="12" />
            <line x1="12" y1="16" x2="12.01" y2="16" />
          </svg>
        </div>
        <h1 style={{ fontSize: "24px", lineHeight: 1.25, margin: "0 0 12px", fontWeight: 700 }}>{copy.title}</h1>
        <p style={{ fontSize: "16px", lineHeight: 1.6, margin: "0 0 8px" }}>{copy.lead}</p>
        <p style={{ fontSize: "14px", lineHeight: 1.6, color: "var(--muted, #a1a1b3)", margin: "0 0 28px" }}>{copy.hint}</p>
        <div style={{ display: "flex", gap: "12px", justifyContent: "center", flexWrap: "wrap" }}>
          <button
            type="button"
            onClick={() => window.location.reload()}
            style={{
              cursor: "pointer",
              border: "1px solid transparent",
              borderRadius: "12px",
              padding: "12px 22px",
              fontSize: "15px",
              fontWeight: 600,
              background: "var(--accent, #f5b301)",
              color: "#16161d",
            }}
          >
            {copy.retry}
          </button>
          <Link
            to="/"
            style={{
              border: "1px solid var(--border, rgba(255,255,255,.08))",
              borderRadius: "12px",
              padding: "12px 22px",
              fontSize: "15px",
              fontWeight: 600,
              textDecoration: "none",
              color: "var(--text, #f4f4f7)",
            }}
          >
            {copy.home}
          </Link>
        </div>
      </main>
    </section>
  );
}
