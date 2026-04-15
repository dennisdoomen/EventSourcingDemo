'use client';

import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import Link from 'next/link';

export function Navigation() {
  return (
    <AppBar position="static">
      <Toolbar>
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          ClubAdmin
        </Typography>
        <Button color="inherit" component={Link} href="/members">
          Members
        </Button>
        <Button color="inherit" component={Link} href="/finances">
          Finances
        </Button>
      </Toolbar>
    </AppBar>
  );
}
