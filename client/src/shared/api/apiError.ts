import axios from "axios";
import type { ApiErrorResponse } from "./apiTypes";

export function getApiErrorMessage(error: unknown, fallbackMessage: string) {
  if (axios.isAxiosError<ApiErrorResponse | { Message?: string; title?: string } | string>(error)) {
    const responseData = error.response?.data;

    if (typeof responseData === "string" && responseData.trim()) {
      return responseData;
    }

    if (responseData && typeof responseData === "object") {
      const message =
        "message" in responseData
          ? responseData.message
          : "Message" in responseData
            ? responseData.Message
            : "title" in responseData
              ? responseData.title
              : null;

      if (message) {
        return message;
      }
    }
  }

  return fallbackMessage;
}
