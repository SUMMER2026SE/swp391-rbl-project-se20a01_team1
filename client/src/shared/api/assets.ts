import { httpClient } from "./httpClient";

const apiOrigin = (httpClient.defaults.baseURL ?? "").replace(/\/api\/?$/, "");

export function toAssetUrl(value?: string | null) {
  if (!value) {
    return "";
  }

  if (
    value.startsWith("http://") ||
    value.startsWith("https://") ||
    value.startsWith("blob:") ||
    value.startsWith("data:")
  ) {
    return value;
  }

  if (value.startsWith("/uploads/")) {
    return `${apiOrigin}${value}`;
  }

  return `${apiOrigin}/uploads/${value.replace(/^\/?uploads\//, "")}`;
}
