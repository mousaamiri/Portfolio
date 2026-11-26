import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";

const THEME_KEY = "portfolio-theme";
const ACCENT_KEY = "portfolio-accent";

export const ACCENTS = [
  { name: "amber", color: "#f0a63b", soft: "rgba(240,166,59,0.14)", strong: "#ffc069" },
  { name: "fuchsia", color: "#d946ef", soft: "rgba(217,70,239,0.14)", strong: "#e879f9" },
  { name: "indigo", color: "#5b6bff", soft: "rgba(91,107,255,0.14)", strong: "#818cf8" },
  { name: "pink", color: "#ec4899", soft: "rgba(236,72,153,0.14)", strong: "#f472b6" },
  { name: "emerald", color: "#10d9a3", soft: "rgba(16,217,163,0.14)", strong: "#34d399" },
];

const ThemeContext = createContext(null);

function readTheme() {
  try {
    const saved = localStorage.getItem(THEME_KEY);
    if (saved === "light" || saved === "dark") return saved;
  } catch {
    /* noop */
  }
  return window.matchMedia("(prefers-color-scheme: light)").matches ? "light" : "dark";
}

function readAccent() {
  try {
    const saved = localStorage.getItem(ACCENT_KEY);
    if (ACCENTS.some((a) => a.name === saved)) return saved;
  } catch {
    /* noop */
  }
  return "amber";
}

function applyAccent(name) {
  const accent = ACCENTS.find((a) => a.name === name) || ACCENTS[0];
  const root = document.documentElement;
  root.style.setProperty("--accent", accent.color);
  root.style.setProperty("--accent-soft", accent.soft);
  root.style.setProperty("--accent-strong", accent.strong);
}

export function ThemeProvider({ children }) {
  const [theme, setTheme] = useState(readTheme);
  const [accent, setAccentState] = useState(readAccent);

  useEffect(() => {
    document.documentElement.setAttribute("data-theme", theme);
    try {
      localStorage.setItem(THEME_KEY, theme);
    } catch {
      /* noop */
    }
  }, [theme]);

  useEffect(() => {
    applyAccent(accent);
    try {
      localStorage.setItem(ACCENT_KEY, accent);
    } catch {
      /* noop */
    }
  }, [accent]);

  const toggleTheme = useCallback(() => {
    setTheme((cur) => (cur === "dark" ? "light" : "dark"));
  }, []);

  const setAccent = useCallback((name) => {
    if (!ACCENTS.some((a) => a.name === name)) return;
    const root = document.documentElement;
    root.classList.add("accent-transitioning");
    setTimeout(() => root.classList.remove("accent-transitioning"), 600);
    setAccentState(name);
  }, []);

  const value = useMemo(
    () => ({ theme, isDark: theme === "dark", toggleTheme, accent, setAccent, accents: ACCENTS }),
    [theme, toggleTheme, accent, setAccent]
  );

  return <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>;
}

export function useTheme() {
  const ctx = useContext(ThemeContext);
  if (!ctx) throw new Error("useTheme must be used within ThemeProvider");
  return ctx;
}
