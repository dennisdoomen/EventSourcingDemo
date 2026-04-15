import type { Metadata } from 'next';
import { TransactionList } from '@/modules/finances/TransactionList';

export const metadata: Metadata = { title: 'Finances – ClubAdmin' };

export default function FinancesPage() {
  return <TransactionList />;
}
