import http from 'k6/http';
import { check } from 'k6';

export const options = {
  scenarios: {
    concurrent_lock_test: {
      executor: 'shared-iterations',
      vus: 20,        // 20 "пользователей"
      iterations: 20, // всего 20 запросов, поровну на всех — т.е. одновременно
      maxDuration: '120s',
    },
  },
};

export default function () {
  const res = http.get('https://localhost:7196/debug/rate');

  check(res, {
    'status is 200 or 409': (r) => r.status === 200 || r.status === 409,
  });

  console.log(`VU ${__VU}: status=${res.status}`);
}