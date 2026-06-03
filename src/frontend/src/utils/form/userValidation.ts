import type { ApiTokenModel } from '@/models/ApiToken';
import type { UserListModel } from '@/models/User';
import {
  type ApiTokenStore,
  type TeamStore,
  usePluginStore,
  useUserStore,
} from '@/store';
import type { Rule } from 'ant-design-vue/es/form';

const userStore = useUserStore();
const pluginStore = usePluginStore();

export const hasEightCharacters = (_rule: Rule, value: string) => {
  if (value == '' || value.length >= 8) {
    return Promise.resolve();
  }
  return Promise.reject(new Error('Please insert at least 8 characters.'));
};

export const hasSpecialCharacter = (_rule: Rule, value: string) => {
  const specialCharRegex = /(?=.*[^A-Za-z0-9])/;
  if (value == '' || specialCharRegex.test(value)) {
    return Promise.resolve();
  }
  return Promise.reject(new Error('Please insert a special character.'));
};

export const hasDigit = (_rule: Rule, value: string) => {
  const digitRegex = /(?=.*\d)/;
  if (value == '' || digitRegex.test(value)) {
    return Promise.resolve();
  }
  return Promise.reject(new Error('Please insert a digit.'));
};

export const isANumber = (_rule: Rule, value: string) => {
  const digitRegex = /^\d{1,9}$/;
  const isNumber = digitRegex.test(value);
  const isSafeInteger = Number.isSafeInteger(Number(value));

  if (value == '' || (isNumber && isSafeInteger)) {
    return Promise.resolve();
  }

  return Promise.reject(
    new Error('Please insert a valid number with up to 9 digits.'),
  );
};

export const hasUpperCaseLetter = (_rule: Rule, value: string) => {
  const upperCaseRegex = /(?=.*[A-Z])/;
  if (value == '' || upperCaseRegex.test(value)) {
    return Promise.resolve();
  }
  return Promise.reject(new Error('Please insert an upper case letter.'));
};

export const hasLowerCaseLetter = (_rule: Rule, value: string) => {
  const lowerCaseRegex = /(?=.*[a-z])/;
  if (value == '' || lowerCaseRegex.test(value)) {
    return Promise.resolve();
  }
  return Promise.reject(new Error('Please insert a lower case letter.'));
};

// Creates a regex for all possible E-Mail addresses and checks if the given one fits the pattern
export const isValidEmail = (_rule: Rule, value: string) => {
  const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

  if (value && emailRegex.test(value)) {
    return Promise.resolve();
  } else {
    return Promise.reject(new Error('Please enter a valid email.'));
  }
};

// Checks if any other users has the same email address
export const isUniqueEmail = (_rule: Rule, value: string) => {
  const users: UserListModel[] = userStore.getUsers;

  if (users?.every((user) => user.userName !== value)) {
    return Promise.resolve();
  }
  return Promise.reject(new Error('This email is already in use.'));
};

export const isUniqueEmployeeNr = (_rule: Rule, value: string) => {
  const users: UserListModel[] = userStore.getUsers;
  if (users?.every((user) => user.externalId !== value)) {
    return Promise.resolve();
  }
  return Promise.reject(new Error('This Employee Nr is already in use.'));
};

export const isUniqueUrl = (_rule: Rule, value: string) => {
  const plugins = pluginStore.getPlugins;

  if (plugins?.every((plugin) => plugin.url !== value)) {
    return Promise.resolve();
  }
  return Promise.reject(new Error('This Url is already in use.'));
};

export const CreateIsUniqueTeamName = (teamStore: TeamStore) => {
  return (_rule: Rule, value: string) => {
    const teams = teamStore.getIdToName(value);
    if (teams != undefined) {
      return Promise.reject(new Error('This Team Name is already in use.'));
    }
    return Promise.resolve();
  };
};

export const CreateisUniqueTokenName = (apiTokenStore: ApiTokenStore) => {
  return (_rule: Rule, value: string) => {
    const tokens: ApiTokenModel[] = apiTokenStore.getApiTokens;
    if (tokens?.every((token) => token.name !== value)) {
      return Promise.resolve();
    }
    return Promise.reject(new Error('This Token Name is already in use.'));
  };
};

export const CreateOnlyOneScimToken = (apiTokenStore: ApiTokenStore) => {
  return (_rule: Rule, value: Array<string>) => {
    const tokens: ApiTokenModel[] = apiTokenStore.getApiTokens;
    if (
      value.includes('SCIM') &&
      tokens?.some((token) => token.scopes.includes('SCIM'))
    ) {
      return Promise.reject(new Error('This Token Name is already in use.'));
    }
    return Promise.resolve();
  };
};
