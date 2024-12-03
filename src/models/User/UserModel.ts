export type UserListModel = {
  id: number;
  email: string;
};

export type UserModel = UserListModel & {
  email: string;
};
