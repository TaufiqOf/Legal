export interface User {
  id?: string;
  name: string;
  userName: string;
  email?: string;
}

export interface LoginRequest {
  userName: string;
  password: string;
  moduleName: number;
}

export interface RegistrationRequest {
  name: string;
  userName: string;
  password: string;
  moduleName: number;
}

export interface AuthResponse {
  success: boolean;
  result: {
    id: string;
    name: string;
    userName: string;
    token: string;
  };
  message?: string;
  errors?: string[];
}

export interface JwtPayload {
  UserId: string;
  Name: string;
  UserName: string;
  exp: number;
  iat: number;
}
