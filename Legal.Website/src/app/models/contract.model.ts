export interface Contract {
  id: string;
  author: string;
  name: string;
  description: string;
  created: Date;
  updated?: Date;
}

export interface ContractParameterModel {
  id: string;
  author: string;
  name: string;
  description: string;
  created: Date;
  updated?: Date;
  moduleName: number;
}

export interface PagedResponse<T> {
  data: T[];
  count: number;
  pageNumber: number;
  pageSize: number;
  totalPage: number;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: string[];
}

export interface GetItemsParameterModel {
  pageNumber: number;
  pageSize: number;
  moduleName: number;
}

export interface IdParameterModel {
  id: string;
  moduleName: number;
}
