export type UserListModel = {
  id: number;
};

export type UserModel = UserListModel & {
  email: string;
};
