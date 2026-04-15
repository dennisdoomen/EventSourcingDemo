'use client';

import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import Box from '@mui/material/Box';

interface DataTableProps<T extends Record<string, unknown>> {
  rows: T[];
  columns: GridColDef[];
  loading?: boolean;
}

export function DataTable<T extends Record<string, unknown>>({
  rows,
  columns,
  loading = false,
}: DataTableProps<T>) {
  return (
    <Box sx={{ height: 600, width: '100%' }}>
      <DataGrid
        rows={rows}
        columns={columns}
        loading={loading}
        pageSizeOptions={[25, 50, 100]}
        disableRowSelectionOnClick
      />
    </Box>
  );
}
