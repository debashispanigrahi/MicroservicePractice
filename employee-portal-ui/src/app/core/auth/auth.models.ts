export interface LoginRequest {
  userName: string;
  password: string;
}

export interface AuthToken {
  accessToken: string;
  expiresAtUtc: string;
}

export interface LoginResponse {
  login: AuthToken;
}