import type { Metadata } from 'next';
import { MemberList } from '@/modules/members/MemberList';

export const metadata: Metadata = { title: 'Members – ClubAdmin' };

export default function MembersPage() {
  return <MemberList />;
}
