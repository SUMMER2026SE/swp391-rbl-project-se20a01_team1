import { httpClient } from "../../shared/api/httpClient";
import type { Province, Ward } from "./types";

export async function getProvinces() {
  const response = await httpClient.get<Province[]>("/administrative/provinces");
  return response.data;
}

export async function getWardsByProvince(provinceCode: string) {
  const response = await httpClient.get<Ward[]>(
    `/administrative/provinces/${provinceCode}/wards`,
  );

  return response.data;
}
