'use client';

import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import { DataTable } from '@/components/shared/DataTable';
import type { GridColDef } from '@mui/x-data-grid';

const columns: GridColDef[] = [
  { field: 'id', headerName: 'ID', width: 220 },
  { field: 'transactionDate', headerName: 'Date', width: 130 },
  { field: 'amount', headerName: 'Amount', width: 130, type: 'number' },
  { field: 'currencyCode', headerName: 'Currency', width: 100 },
  { field: 'description', headerName: 'Description', width: 300, flex: 1 },
  { field: 'bookingCode', headerName: 'Booking Code', width: 160 },
  { field: 'isCategorized', headerName: 'Categorized', width: 130, type: 'boolean' },
];

export function TransactionList() {
  // TODO: Fetch from /api/transactions (Finances API)
  const rows: Record<string, unknown>[] = [];

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Financial Transactions
      </Typography>
      <DataTable rows={rows} columns={columns} />
    </Box>
  );
}
