export type ApiResponse<T> = {
  success: boolean;
  message: string;
  data: T;
};

export type ApiErrorResponse = {
  success: false;
  errorCode: string;
  message: string;
  details?: unknown;
};
