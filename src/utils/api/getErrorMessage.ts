class ResponseError extends Error {
  override name: 'ResponseError' = 'ResponseError' as const;
  constructor(
    public response: Response,
    msg?: string,
  ) {
    super(msg);
  }
}

export const getFetchErrorMessage = async (
  error: unknown,
  defaultMessage: string | undefined = 'Unbekannter Fehler',
): Promise<string> => {
  if (error instanceof ResponseError) {
    return await error.response
      ?.json()
      .then((json: { message: string }) => json.message);
  }
  if (error instanceof Error) {
    return error.message;
  }
  if (!error) return defaultMessage;
  return JSON.stringify(error);
};
