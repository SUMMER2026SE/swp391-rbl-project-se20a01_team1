import axios from "axios";

export const httpClient = axios.create({
  baseURL: "http://localhost:5294/api",
});

httpClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("accessToken");

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});
