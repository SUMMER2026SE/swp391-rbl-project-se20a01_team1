import { httpClient } from "../../shared/api/httpClient";
import type { ApiResponse } from "../../shared/api/apiTypes";
import type { CurrentUser } from "./types";

export async function getCurrentUser() {
  const response = await httpClient.get<ApiResponse<CurrentUser>>("/users/me");
  return response.data.data;
}
