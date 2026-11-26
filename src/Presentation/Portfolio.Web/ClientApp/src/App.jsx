import { useEffect } from "react";
import { BrowserRouter, Route, Routes, useLocation } from "react-router-dom";
import { ThemeProvider } from "./lib/theme";
import { LanguageProvider } from "./lib/i18n";

import Layout from "./components/Layout";
import Home from "./pages/Home";
import About from "./pages/About";
import Experience from "./pages/Experience";
import Work from "./pages/Work";
import ProjectDetail from "./pages/ProjectDetail";
import Blog from "./pages/Blog";
import Contact from "./pages/Contact";
import Login from "./pages/Login";
import ForgotPassword from "./pages/ForgotPassword";
import ErrorPage from "./pages/ErrorPage";

import AdminLayout from "./admin/AdminLayout";
import ProtectedRoute from "./admin/ProtectedRoute";
import AdminLogin from "./admin/AdminLogin";
import AdminForgotPassword from "./admin/AdminForgotPassword";
import Dashboard from "./admin/Dashboard";
import AdminList from "./admin/AdminList";
import EntityForm from "./admin/EntityForm";
import Messages from "./admin/Messages";
import MessageView from "./admin/MessageView";
import ProfileForm from "./admin/ProfileForm";
import ChangePassword from "./admin/ChangePassword";

function ScrollToTop() {
  const { pathname } = useLocation();
  useEffect(() => {
    window.scrollTo(0, 0);
  }, [pathname]);
  return null;
}

export default function App() {
  return (
    <ThemeProvider>
      <LanguageProvider>
        <BrowserRouter>
          <ScrollToTop />
          <Routes>
            <Route element={<Layout />}>
              <Route index element={<Home />} />
              <Route path="/About" element={<About />} />
              <Route path="/Experience" element={<Experience />} />
              <Route path="/Work" element={<Work />} />
              <Route path="/Work/:slug" element={<ProjectDetail />} />
              <Route path="/Blog" element={<Blog />} />
              <Route path="/Contact" element={<Contact />} />
            </Route>

            <Route path="/Login" element={<Login />} />
            <Route path="/ForgotPassword" element={<ForgotPassword />} />

            <Route path="/Admin/Login" element={<AdminLogin />} />
            <Route path="/Admin/ForgotPassword" element={<AdminForgotPassword />} />

            <Route path="/Admin" element={<ProtectedRoute />}>
              <Route element={<AdminLayout />}>
                <Route index element={<Dashboard />} />
                <Route path="Profile" element={<ProfileForm />} />
                <Route path="ChangePassword" element={<ChangePassword />} />
                <Route path="Messages" element={<Messages />} />
                <Route path="Messages/:id" element={<MessageView />} />
                <Route path=":resource" element={<AdminList />} />
                <Route path=":resource/new" element={<EntityForm />} />
                <Route path=":resource/:id/edit" element={<EntityForm />} />
              </Route>
            </Route>

            <Route path="*" element={<ErrorPage status={404} />} />
          </Routes>
        </BrowserRouter>
      </LanguageProvider>
    </ThemeProvider>
  );
}
