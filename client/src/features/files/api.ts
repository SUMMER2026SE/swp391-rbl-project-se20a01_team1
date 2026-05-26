import { httpClient } from "../../shared/api/httpClient";
import type { ApiResponse } from "../../shared/api/apiTypes";

export type FileUploadScope = "RoomingHouse" | "Room" | "LegalDocument";

export type FileUploadResult = {
  objectKey: string;
  url: string;
};

export async function uploadImage(file: File, scope: FileUploadScope) {
  const formData = new FormData();
  formData.append("file", file);
  formData.append("scope", scope);

  const response = await httpClient.post<ApiResponse<FileUploadResult>>(
    "/files/images",
    formData,
    {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    },
  );

  return response.data.data;
}
