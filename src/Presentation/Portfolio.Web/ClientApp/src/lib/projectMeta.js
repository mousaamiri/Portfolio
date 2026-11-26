// Presentation-level derivation of case-study metadata from the Project DTO.
// The API exposes no category / role / duration columns, so the UI derives
// stable labels from real fields (technologies, dates, flags) instead of
// inventing content.

const CATEGORY_RULES = [
  { key: "desktop", test: /wpf|winforms|winui|\bmaui\b|desktop/i },
  { key: "product", test: /\bddd\b|domain-driven|cqrs|outbox|microservice|event[- ]sourc/i },
  { key: "web", test: /asp\.net|blazor|razor|react|next\.js|vue|angular|node|express|django|flask|laravel/i },
];

// Order used by the filter row on the list page.
export const CATEGORY_KEYS = ["web", "product", "desktop"];

const FA_DIGITS = "۰۱۲۳۴۵۶۷۸۹";
const MONTH_MS = 1000 * 60 * 60 * 24 * 30.44;

export function isRtlLang(lang) {
  return lang === "fa" || lang === "ar";
}

export function localizeDigits(value, lang) {
  const text = String(value ?? "");
  if (!isRtlLang(lang)) return text;
  return text.replace(/\d/g, (digit) => FA_DIGITS[Number(digit)]);
}

export function pad2(value) {
  return String(value).padStart(2, "0");
}

export function projectCategory(project) {
  const tech = project?.technologies || "";
  for (const rule of CATEGORY_RULES) {
    if (rule.test.test(tech)) return rule.key;
  }
  return "web";
}

export function projectYear(project, lang) {
  const iso = project?.completedAt || project?.startedAt;
  if (!iso) return null;
  const date = new Date(iso);
  // Unset DateTime columns serialize as 0001-01-01; treat them as "no date".
  if (Number.isNaN(date.getTime()) || date.getUTCFullYear() < 1900) return null;
  if (isRtlLang(lang)) {
    return new Intl.DateTimeFormat("fa-IR-u-ca-persian", { year: "numeric" }).format(date);
  }
  return String(date.getUTCFullYear());
}

// Returns { months, ongoing } or null when the start date is unusable.
export function projectDuration(project) {
  const start = project?.startedAt ? new Date(project.startedAt) : null;
  if (!start || Number.isNaN(start.getTime()) || start.getUTCFullYear() < 1900) return null;

  const end = project?.completedAt ? new Date(project.completedAt) : null;
  if (!end || Number.isNaN(end.getTime()) || end < start) {
    return { months: null, ongoing: true };
  }
  return { months: Math.max(1, Math.round((end - start) / MONTH_MS)), ongoing: false };
}

export function splitParagraphs(text) {
  if (!text) return [];
  return String(text)
    .split(/\r?\n\s*\r?\n/)
    .map((paragraph) => paragraph.trim())
    .filter(Boolean);
}

export function projectPath(slug) {
  return `/Work/${encodeURIComponent(slug || "")}`;
}
