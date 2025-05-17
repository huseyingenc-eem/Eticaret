import React from 'react';
import { Box, Button, Card, CardActions, CardContent, CardMedia, Container, Typography } from '@mui/material';
import Grid from '@mui/material/Grid';
import ImageSlider from '../components/common/ImageSlider';

const HomePage: React.FC = () => {
    return (
        <>
            <Box sx={{ width: '100%', position: 'relative' }}>
                {/* Carousel common */}
                <ImageSlider
                    images={[
                        'https://picsum.photos/1200/600',
                        'https://picsum.photos/1200/599',
                        'https://picsum.photos/1200/601',
                    ]}
                />
            </Box>


            {/* Hizmetler, Ulaşım, İletişim */}
            <Container sx={{ my: 6 }}>
                <Grid container spacing={4}>
                    {[
                        { title: 'Hizmetler', desc: 'Lorem ipsum dolor sit amet consectetur adipisicing elit.' },
                        { title: 'Ulaşım', desc: 'Deserunt fugiat hic tempora ullam asperiores iusto.' },
                        { title: 'İletişim', desc: 'In aut ut dolores doloribus id voluptatem.' }
                    ].map((item, i) => (
                        <Grid item xs={12} md={4} key={i}>
                            <Typography variant="h5" align="center" gutterBottom>{item.title}</Typography>
                            <Typography align="center" paragraph>{item.desc}</Typography>
                            <Box textAlign="center">
                                <Button variant="text">Devamı</Button>
                            </Box>
                        </Grid>
                    ))}
                </Grid>
            </Container>

            {/* Gri Arka Planlı Tanıtım */}
            <Box sx={{ bgcolor: '#f5f5f5', py: 8 }}>
                <Container>
                    <Grid container spacing={4} alignItems="center">
                        <Grid item xs={12} md={6}>
                            <Box
                                component="img"
                                src="https://picsum.photos/375/430"
                                sx={{ width: '100%', borderRadius: 4 }}
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <Typography variant="h4" gutterBottom>GOSB TEKNOPARK</Typography>
                            <Typography paragraph>
                                Lorem ipsum dolor sit, amet consectetur adipisicing elit. Rem placeat id accusamus sit.
                            </Typography>
                            <Typography paragraph>
                                Rem placeat id accusamus sit ducimus non repellat porro alias incidunt illum.
                            </Typography>
                            <Button variant="contained" color="secondary">Devamını oku</Button>
                        </Grid>
                    </Grid>
                </Container>
            </Box>

            {/* Blog Bölümü */}
            <Container sx={{ my: 8 }}>
                <Typography variant="h4" align="center" gutterBottom>BLOG</Typography>
                <Typography align="center" paragraph>
                    Lorem, ipsum dolor sit amet consectetur adipisicing elit. Aliquam, maxime?
                </Typography>
                <Grid container spacing={4}>
                    {[1, 2, 3].map((item, i) => (
                        <Grid item xs={12} md={4} key={i}>
                            <Card>
                                <CardMedia
                                    component="img"
                                    height="200"
                                    image={`https://picsum.photos/300/20${i}`}
                                    alt="Blog görseli"
                                />
                                <CardContent>
                                    <Typography variant="h6">HTML'in Temelleri</Typography>
                                    <Typography variant="body2" color="text.secondary">
                                        Some quick example text to build on the card title and make up the bulk of the card’s content.
                                    </Typography>
                                </CardContent>
                                <CardActions>
                                    <Button size="small">Yazıyı Oku</Button>
                                </CardActions>
                            </Card>
                        </Grid>
                    ))}
                </Grid>
            </Container>
        </>
    );
};

export default HomePage;