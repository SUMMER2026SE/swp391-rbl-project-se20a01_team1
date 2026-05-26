import { httpClient } from "../../shared/api/httpClient";
import type { ApiResponse } from "../../shared/api/apiTypes";
import type {
  Amenity,
  PropertyImageRequest,
  RoomingHouseBasicInfoRequest,
  RoomingHouseDetail,
  RoomingHouseOnboarding,
  RoomingHouseSummary,
  UpdateLegalDocumentRequest,
} from "./types";

export async function getMyRoomingHouses() {
  const response =
    await httpClient.get<ApiResponse<RoomingHouseSummary[]>>(
      "/rooming-houses/my",
    );

  return response.data.data;
}

export async function getMyRoomingHouseOnboarding() {
  const response = await httpClient.get<ApiResponse<RoomingHouseOnboarding>>(
    "/rooming-houses/my/onboarding",
  );

  return response.data.data;
}

export async function getAmenities(scope?: "House" | "Room" | "Both") {
  const response = await httpClient.get<ApiResponse<Amenity[]>>("/amenities", {
    params: scope ? { scope } : undefined,
  });

  return response.data.data;
}

export async function getRoomingHouseDetail(id: string) {
  const response = await httpClient.get<ApiResponse<RoomingHouseDetail>>(
    `/rooming-houses/${id}`,
  );

  return response.data.data;
}

export async function createRoomingHouseDraft(
  request: RoomingHouseBasicInfoRequest,
) {
  const response = await httpClient.post<ApiResponse<RoomingHouseDetail>>(
    "/rooming-houses/draft",
    request,
  );

  return response.data.data;
}

export async function updateRoomingHouseBasicInfo(
  id: string,
  request: RoomingHouseBasicInfoRequest,
) {
  const response = await httpClient.put<ApiResponse<RoomingHouseDetail>>(
    `/rooming-houses/${id}`,
    request,
  );

  return response.data.data;
}

export async function updateRoomingHouseImages(
  id: string,
  images: PropertyImageRequest[],
) {
  const response = await httpClient.put<ApiResponse<RoomingHouseDetail>>(
    `/rooming-houses/${id}/images`,
    { images },
  );

  return response.data.data;
}

export async function updateRoomingHouseAmenities(
  id: string,
  amenityIds: number[],
) {
  const response = await httpClient.put<ApiResponse<RoomingHouseDetail>>(
    `/rooming-houses/${id}/amenities`,
    { amenityIds },
  );

  return response.data.data;
}

export async function updateRoomingHouseLegalDocument(
  id: string,
  request: UpdateLegalDocumentRequest,
) {
  const response = await httpClient.put<ApiResponse<RoomingHouseDetail>>(
    `/rooming-houses/${id}/legal-document`,
    request,
  );

  return response.data.data;
}

export async function submitRoomingHouse(id: string) {
  const response = await httpClient.post<ApiResponse<RoomingHouseDetail>>(
    `/rooming-houses/${id}/submit`,
  );

  return response.data.data;
}
