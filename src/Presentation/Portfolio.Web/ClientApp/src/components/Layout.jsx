import { useCallback, useEffect, useState } from "react";
import { Outlet } from "react-router-dom";
import Navbar from "./Navbar";
import Footer from "./Footer";
import CommandPalette from "./CommandPalette";
import ThemeSwitcher from "./ThemeSwitcher";
import ScrollProgress from "./ScrollProgress";

export default function Layout() {
  const [paletteOpen, setPaletteOpen] = useState(false);

  const openPalette = useCallback(() => setPaletteOpen(true), []);
  const closePalette = useCallback(() => setPaletteOpen(false), []);

  useEffect(() => {
    const onKey = (e) => {
      if ((e.key === "k" || e.key === "K") && (e.metaKey || e.ctrlKey)) {
        e.preventDefault();
        setPaletteOpen((v) => !v);
      }
    };
    document.addEventListener("keydown", onKey);
    return () => document.removeEventListener("keydown", onKey);
  }, []);

  return (
    <>
      <Navbar onOpenPalette={openPalette} />
      <main id="page-content">
        <Outlet />
      </main>
      <Footer />
      <ThemeSwitcher />
      <ScrollProgress />
      <CommandPalette open={paletteOpen} onClose={closePalette} />
    </>
  );
}
