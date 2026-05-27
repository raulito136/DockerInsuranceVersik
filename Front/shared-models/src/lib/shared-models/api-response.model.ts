export interface ApiError {
  field: string;
  message: string;
}

export interface ApiResponse<T> {
  data?: T;
  errors: ApiError[];
}

export interface PagedResponse<T> {
  data: T[];
  page: number;
  pageSize: number;
  total: number;
}