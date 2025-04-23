export interface HackerNewsItem {
    id: number;
    by?: string;
    score?: number;
    time: number;
    title?: string;
    type?: string;
    url?: string;
    text?: string;
    deleted?: boolean;
    dead?: boolean;
    parent?: number;
    kids?: number[];
    descendants?: number;
    parts?: number[];
    poll?: number;
  }
  