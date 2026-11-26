// Presentation-owned technology → pill accent color (port of TechColor.cs).
const DEFAULT = "#3b82f6";

const MAP = {
  java: "#f0a63b",
  "java 21": "#f0a63b",
  python: "#f0a63b",
  jwt: "#f0a63b",
  "spring boot": "#22c55e",
  "spring ai": "#22c55e",
  "spring security": "#22c55e",
  "jwt auth": "#22c55e",
  ".net": "#22c55e",
  "asp.net core": "#22c55e",
  "tailwind css": "#14b8a6",
  "apache pdfbox": "#ec4899",
  "chart.js": "#ec4899",
  redis: "#ec4899",
  "tmdb api": "#ec4899",
  pgvector: "#a855f7",
  websocket: "#a855f7",
  pandas: "#a855f7",
};

export function techColor(name) {
  return MAP[String(name).trim().toLowerCase()] || DEFAULT;
}

export function splitTechs(technologies) {
  if (!technologies) return [];
  return technologies
    .split(",")
    .map((s) => s.trim())
    .filter(Boolean)
    .map((name) => ({ name, color: techColor(name) }));
}

export function modernImageSrc(src, format) {
  if (!src) return "";
  const dot = src.lastIndexOf(".");
  const slash = src.lastIndexOf("/");
  if (dot <= slash) return "";
  return `${src.slice(0, dot)}.${format}`;
}
