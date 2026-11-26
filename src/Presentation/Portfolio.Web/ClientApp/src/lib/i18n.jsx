import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
import { api } from "./api";

const LANG_KEY = "portfolio-lang";
const SUPPORTED = ["en", "fa", "ar"];
const RTL = new Set(["fa", "ar"]);

const LanguageContext = createContext(null);

// UI-text rows were authored through the former Razor admin forms, so some
// values are stored HTML-encoded ("&amp;", "&lt;br /&gt;", ...). Razor used to
// render them raw; React escapes text nodes, which turned "&amp;" into a literal
// "&amp;" on screen. Decode the standard entities once when a value is read.
const ENTITIES = {
  "&amp;": "&",
  "&lt;": "<",
  "&gt;": ">",
  "&quot;": '"',
  "&#39;": "'",
  "&apos;": "'",
  "&nbsp;": "\u00a0",
};

function decodeEntities(value) {
  if (typeof value !== "string" || value.indexOf("&") === -1) return value;
  return value.replace(/&(amp|lt|gt|quot|#39|apos|nbsp);/g, (m) => ENTITIES[m] ?? m);
}

function readStoredLang() {
  try {
    const saved = localStorage.getItem(LANG_KEY);
    if (SUPPORTED.includes(saved)) return saved;
  } catch {
    /* noop */
  }
  return "en";
}

function persistLang(lang) {
  try {
    localStorage.setItem(LANG_KEY, lang);
  } catch {
    /* noop */
  }
  // Keep the cookie for parity with the former server-side language switch.
  document.cookie = `portfolio-lang=${lang}; path=/; max-age=${60 * 60 * 24 * 365}; samesite=lax`;
}

export function LanguageProvider({ children }) {
  const [lang, setLangState] = useState(readStoredLang);
  const [ui, setUi] = useState({});

  const dir = RTL.has(lang) ? "rtl" : "ltr";

  useEffect(() => {
    document.documentElement.setAttribute("lang", lang);
    document.documentElement.setAttribute("dir", dir);
    persistLang(lang);
  }, [lang, dir]);

  useEffect(() => {
    let active = true;
    api
      .getUiTranslations(lang)
      .then((map) => {
        if (active) setUi(map || {});
      })
      .catch(() => {
        if (active) setUi({});
      });
    return () => {
      active = false;
    };
  }, [lang]);

  const t = useCallback(
    (key, fallback) => {
      if (ui && ui[key]) return decodeEntities(ui[key]);
      return fallback !== undefined ? fallback : key;
    },
    [ui]
  );

  const setLang = useCallback((next) => {
    const resolved = SUPPORTED.includes(next) ? next : "en";
    setLangState(resolved);
  }, []);

  const value = useMemo(
    () => ({ lang, dir, isRtl: dir === "rtl", t, setLang, supported: SUPPORTED }),
    [lang, dir, t, setLang]
  );

  return <LanguageContext.Provider value={value}>{children}</LanguageContext.Provider>;
}

export function useI18n() {
  const ctx = useContext(LanguageContext);
  if (!ctx) throw new Error("useI18n must be used within LanguageProvider");
  return ctx;
}
