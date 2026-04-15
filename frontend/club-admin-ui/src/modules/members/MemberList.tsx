'use client';

import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import { DataTable } from '@/components/shared/DataTable';
import type { GridColDef } from '@mui/x-data-grid';

const columns: GridColDef[] = [
  { field: 'id', headerName: 'ID', width: 220 },
  { field: 'firstName', headerName: 'First Name', width: 150 },
  { field: 'lastName', headerName: 'Last Name', width: 150 },
  { field: 'membershipType', headerName: 'Type', width: 120 },
  { field: 'contributionCategory', headerName: 'Category', width: 130 },
  { field: 'isActive', headerName: 'Active', width: 100, type: 'boolean' },
];

export function MemberList() {
  // TODO: Fetch from /api/members (Members API)
  const rows: Record<string, unknown>[] = [];

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Members
      </Typography>
      <DataTable rows={rows} columns={columns} />
    </Box>
  );
}
