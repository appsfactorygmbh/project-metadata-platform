import { extractToken } from '../extractToken';

describe('extractToken', () => {
  it('should extract the token from and Bearer string', () => {
    expect(extractToken('Bearer 123')).toBe('123');
  });

  it('should extract the token from and Refresh string', () => {
    expect(extractToken('Refresh 123')).toBe('123');
  });

  it('should extract the token from a string without Bearer or Refresh', () => {
    expect(extractToken('123')).toBe('123');
  });
});
