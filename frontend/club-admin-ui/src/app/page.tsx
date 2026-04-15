import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';
import Link from 'next/link';

export default function HomePage() {
  return (
    <Box sx={{ p: 4 }}>
      <Typography variant="h3" gutterBottom>
        Welcome to ClubAdmin
      </Typography>
      <Typography variant="body1" color="text.secondary" paragraph>
        Manage your sports club members and finances from one place.
      </Typography>
      <Box sx={{ display: 'flex', gap: 2, mt: 3 }}>
        <Button variant="contained" component={Link} href="/members">
          Manage Members
        </Button>
        <Button variant="outlined" component={Link} href="/finances">
          Financial Transactions
        </Button>
      </Box>
    </Box>
  );
}
