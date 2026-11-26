// Mirrors the server-side Features:* configuration flags.
export const FEATURES = {
  blogEnabled: import.meta.env.VITE_BLOG_ENABLED !== "false",
};

export const ROUTES = {
  home: "/",
  about: "/About",
  experience: "/Experience",
  work: "/Work",
  workDetail: "/Work/:slug",
  blog: "/Blog",
  contact: "/Contact",
};

// Maps a pathname to the navbar page key used for the active pill.
export function pageFromPath(pathname) {
  const path = pathname.replace(/\/+$/, "") || "/";
  if (path === "/") return "home";
  // Nested routes (e.g. /Work/vitastic) keep their parent section active.
  const key = path.slice(1).split("/")[0].toLowerCase();
  if (["about", "experience", "work", "blog", "contact"].includes(key)) return key;
  return "";
}
