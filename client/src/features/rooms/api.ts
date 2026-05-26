import { httpClient } from "../../shared/api/httpClient";
import type { ApiResponse } from "../../shared/api/apiTypes";
import type { PropertyImageRequest } from "../rooming-houses/types";
import type { CreateRoomRequest, Room, RoomPriceTierRequest } from "./types";

export async function createRoom(
  roomingHouseId: string,
  request: CreateRoomRequest,
) {
  const response = await httpClient.post<ApiResponse<Room>>(
    `/rooming-houses/${roomingHouseId}/rooms`,
    request,
  );

  return response.data.data;
}

export async function getRoomsByRoomingHouse(roomingHouseId: string) {
  const response = await httpClient.get<ApiResponse<Room[]>>(
    `/rooming-houses/${roomingHouseId}/rooms`,
  );

  return response.data.data;
}

export async function getRoomDetail(id: string) {
  const response = await httpClient.get<ApiResponse<Room>>(`/rooms/${id}`);
  return response.data.data;
}

export async function updateRoom(id: string, request: CreateRoomRequest) {
  const response = await httpClient.put<ApiResponse<Room>>(
    `/rooms/${id}`,
    request,
  );

  return response.data.data;
}

export async function updateRoomImages(
  id: string,
  images: PropertyImageRequest[],
) {
  const response = await httpClient.put<ApiResponse<Room>>(
    `/rooms/${id}/images`,
    { images },
  );

  return response.data.data;
}

export async function updateRoomAmenities(id: string, amenityIds: number[]) {
  const response = await httpClient.put<ApiResponse<Room>>(
    `/rooms/${id}/amenities`,
    { amenityIds },
  );

  return response.data.data;
}

export async function updateRoomPriceTiers(
  id: string,
  priceTiers: RoomPriceTierRequest[],
) {
  const response = await httpClient.put<ApiResponse<Room>>(
    `/rooms/${id}/price-tiers`,
    { priceTiers },
  );

  return response.data.data;
}

export async function submitRoom(id: string) {
  const response = await httpClient.post<ApiResponse<Room>>(
    `/rooms/${id}/submit`,
  );

  return response.data.data;
}
