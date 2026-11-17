import { Field } from './field';

export interface PageFilter {
  id: string;
  label: string;
  options: { value: string; label: string }[];
  // eslint-disable-next-line @typescript-eslint/no-explicit-any -- vrstica poljubnega entitetnega tipa iz crud-page
  match: (row: Record<string, any>, value: string) => boolean;
}

export interface PageConfig {
  title: string;
  itemName: string;
  path: string;
  fields: Field[];
  sortBy?: string;
  filters?: PageFilter[];
  detail?: string;
  writeRoles: string[];
  empty: () => Record<string, unknown>;
}
