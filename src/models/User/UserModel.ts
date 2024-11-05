export type UserListModel = {
  id: number;
  name: string;
  username: string;
};

export type UserModel = UserListModel & {
  email: string;
};
