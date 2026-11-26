import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// The React SPA is served by the .NET Web host from wwwroot after `npm run build`.
// During development it runs on its own port and talks to Portfolio.API directly.
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    // Same-origin API calls in dev: /api/* is forwarded to Portfolio.API.
    proxy: {
      "/api": {
        target: process.env.VITE_API_PROXY_TARGET || "https://localhost:7003",
        changeOrigin: true,
        secure: false,
      },
    },
  },
  build: {
    outDir: "../wwwroot",
    emptyOutDir: false,
    assetsDir: "app",
  },
});
