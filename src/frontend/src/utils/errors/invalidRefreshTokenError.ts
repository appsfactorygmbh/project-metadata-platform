export class InvalidRefreshTokenError extends Error {
  constructor(message = 'Invalid refresh token.') {
    super(message);
    this.name = 'InvalidRefreshTokenError';
  }
}
