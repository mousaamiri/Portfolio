// Thin client over Portfolio.API. Mirrors the endpoints the MVC Web layer used:
// public reads under /api/public/*, authenticated admin CRUD under /api/admin/*.
// Everything is wrapped in the API's ApiResponse<T> envelope ({ success, data }).

// Empty by default: the SPA calls /api/* on its own origin and the Web host
// forwards to Portfolio.API (see Program.cs). Set VITE_API_BASE_URL only when
// the SPA must talk to a different API origin directly.
const BASE = (import.meta.env.VITE_API_BASE_URL ?? "").replace(/\/+$/, "");

const TOKEN_KEY = "portfolio-admin-token";
const EXPIRES_KEY = "portfolio-admin-expires";

export class ApiError extends Error {
  constructor(status, message) {
    super(message || `Request failed (${status})`);
    this.status = status;
  }
}

export function getToken() {
  try {
    const token = localStorage.getItem(TOKEN_KEY);
    const expires = localStorage.getItem(EXPIRES_KEY);
    if (token && expires && new Date(expires).getTime() < Date.now()) {
      clearToken();
      return null;
    }
    return token;
  } catch {
    return null;
  }
}

export function setToken(token, expiresAt) {
  try {
    localStorage.setItem(TOKEN_KEY, token);
    if (expiresAt) localStorage.setItem(EXPIRES_KEY, expiresAt);
  } catch {
    /* storage unavailable */
  }
}

export function clearToken() {
  try {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(EXPIRES_KEY);
  } catch {
    /* noop */
  }
}

export function isAuthenticated() {
  return !!getToken();
}

async function request(path, { method = "GET", body, auth = false } = {}) {
  const headers = {};
  if (body !== undefined) headers["Content-Type"] = "application/json";
  if (auth) {
    const token = getToken();
    if (token) headers["Authorization"] = `Bearer ${token}`;
  }

  const res = await fetch(BASE + path, {
    method,
    headers,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  });

  if (res.status === 401 && auth) {
    clearToken();
    window.dispatchEvent(new Event("admin-unauthorized"));
  }
  return res;
}

async function json(path, opts) {
  const res = await request(path, opts);
  if (!res.ok) throw new ApiError(res.status);
  if (res.status === 204) return null;
  return res.json();
}

function withLang(path, lang) {
  return lang ? `${path}${path.includes("?") ? "&" : "?"}lang=${encodeURIComponent(lang)}` : path;
}

/* ── Public reads ─────────────────────────────────────────────────────── */

async function getPublicList(resource, lang) {
  const body = await json(withLang(`/api/public/${resource}`, lang));
  return body?.data ?? [];
}

export const api = {
  getProjects: (lang) => getPublicList("projects", lang),
  getSkills: (lang) => getPublicList("skills", lang),
  getExperiences: (lang) => getPublicList("experiences", lang),
  getEducations: (lang) => getPublicList("educations", lang),
  getArticles: (lang) => getPublicList("articles", lang),
  getFaqs: (lang) => getPublicList("faqs", lang),
  getTimeline: (lang) => getPublicList("timeline", lang),
  getInterests: (lang) => getPublicList("interests", lang),
  getStats: (lang) => getPublicList("stats", lang),
  getImpactMetrics: (lang) => getPublicList("impact-metrics", lang),
  getPrinciples: (lang) => getPublicList("principles", lang),
  getProficiencies: (lang) => getPublicList("proficiencies", lang),
  getTestimonials: (lang) => getPublicList("testimonials", lang),

  async getProfile(lang) {
    try {
      const body = await json(withLang("/api/public/profile", lang));
      return body?.data ?? null;
    } catch (err) {
      if (err instanceof ApiError && err.status >= 400 && err.status < 500) return null;
      throw err;
    }
  },

  async getUiTranslations(lang) {
    try {
      const body = await json(withLang("/api/public/ui-translations", lang));
      return body?.data ?? {};
    } catch (err) {
      if (err instanceof ApiError && err.status >= 400 && err.status < 500) return {};
      throw err;
    }
  },

  async sendMessage(payload) {
    const res = await request("/api/public/messages", { method: "POST", body: payload });
    // Return the status so the contact form can special-case rate limiting (429).
    return { ok: res.ok, status: res.status };
  },

  /* ── Admin auth ─────────────────────────────────────────────────────── */

  async adminLogin(username, password) {
    const res = await request("/api/admin/auth/login", {
      method: "POST",
      body: { username, password },
    });
    if (!res.ok) return null;
    const body = await res.json();
    return body?.data ?? null; // { token, expiresAt }
  },

  async adminChangePassword(currentPassword, newPassword) {
    const res = await request("/api/admin/auth/change-password", {
      method: "POST",
      auth: true,
      body: { currentPassword, newPassword },
    });
    return res.ok;
  },

  /* ── Admin CRUD (generic over api/admin/{resource}) ─────────────────── */

  async adminList(resource, lang = "en") {
    const body = await json(withLang(`/api/admin/${resource}`, lang), { auth: true });
    return body?.data ?? [];
  },

  async adminGet(resource, id, lang = "en") {
    const res = await request(withLang(`/api/admin/${resource}/${id}`, lang), { auth: true });
    if (!res.ok) return null;
    const body = await res.json();
    return body?.data ?? null;
  },

  async adminCreate(resource, payload) {
    const res = await request(`/api/admin/${resource}`, {
      method: "POST",
      auth: true,
      body: payload,
    });
    if (!res.ok) return null;
    const body = await res.json();
    return body?.data ?? null;
  },

  async adminUpdate(resource, id, payload) {
    const res = await request(`/api/admin/${resource}/${id}`, {
      method: "PUT",
      auth: true,
      body: payload,
    });
    return res.ok;
  },

  async adminDelete(resource, id) {
    const res = await request(`/api/admin/${resource}/${id}`, { method: "DELETE", auth: true });
    return res.ok;
  },

  /* ── Admin: messages ────────────────────────────────────────────────── */

  async adminMessages() {
    const body = await json("/api/admin/messages", { auth: true });
    return body?.data ?? [];
  },

  async adminMessage(id) {
    const res = await request(`/api/admin/messages/${id}`, { auth: true });
    if (!res.ok) return null;
    const body = await res.json();
    return body?.data ?? null;
  },

  async adminMarkMessageRead(id) {
    const res = await request(`/api/admin/messages/${id}/read`, { method: "PUT", auth: true });
    return res.ok;
  },

  async adminDeleteMessage(id) {
    const res = await request(`/api/admin/messages/${id}`, { method: "DELETE", auth: true });
    return res.ok;
  },

  /* ── Admin: profile (single upsert) ─────────────────────────────────── */

  async adminUpsertProfile(payload) {
    const res = await request("/api/admin/profile", {
      method: "PUT",
      auth: true,
      body: payload,
    });
    return res.ok;
  },
};
