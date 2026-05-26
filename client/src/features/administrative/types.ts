export type Province = {
  code: string;
  name: string;
  type: string;
};

export type Ward = {
  code: string;
  provinceCode: string;
  name: string;
  type: string;
};
