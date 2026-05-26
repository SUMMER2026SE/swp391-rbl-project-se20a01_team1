import { httpClient } from "../../shared/api/httpClient";
import type { ApiResponse } from "../../shared/api/apiTypes";
import type { LoginRequest, LoginResponse } from "./types";

export async function login(request: LoginRequest) {
  const response = await httpClient.post<ApiResponse<LoginResponse>>(
    "/auth/login",
    request,
  );

  return response.data.data;
}
