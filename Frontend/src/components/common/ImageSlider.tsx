import React from 'react';
import Slider from 'react-slick';
import {Box} from '@mui/material';
import {Icon} from '@iconify/react';

interface ImageSliderProps {
    images: string[]; // doğrudan URL listesi
    customSettings?: object; // react-slick ayarlarını dışarıdan geçebileceğin prop
}

const SampleNextArrow = (props: any) => {
    const {className, onClick} = props;
    return (
        <div
            className={className}
            onClick={onClick}
            style={{
                zIndex: 2,
                right: 10,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                position: 'absolute',
                width: 40,
                height: 40,
                backgroundColor: 'white',
                borderRadius: '50%',
                boxShadow: '0 2px 6px rgba(0,0,0,0.2)',
            }}
        >
            <Icon icon="mdi:chevron-right" width={24}/>
        </div>
    );
};

// Sol ok
const SamplePrevArrow = (props: any) => {
    const {className, onClick} = props;
    return (
        <div
            className={className}
            onClick={onClick}
            style={{
                zIndex: 2,
                left: 10,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                position: 'absolute',
                width: 40,
                height: 40,
                backgroundColor: 'white',
                borderRadius: '50%',
                boxShadow: '0 2px 6px rgba(0,0,0,0.2)',
            }}
        >
            <Icon icon="mdi:chevron-left" width={24}/>
        </div>
    );
};

const ImageSlider: React.FC<ImageSliderProps> = ({images, customSettings = {}}) => {
    const defaultSettings = {
        dots: true,
        infinite: true,
        speed: 500,
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 3000,
        arrows: true,
        nextArrow: <SampleNextArrow/>,
        prevArrow: <SamplePrevArrow/>,
        ...customSettings,
    };


    return (
        <Box sx={{width: '100%'}}>
            <Slider {...defaultSettings}>
                {images.map((url, i) => (
                    <Box
                        key={i}
                        component="img"
                        src={url}
                        alt={`slide-${i}`}
                        sx={{width: '100%', display: 'block'}}
                    />
                ))}
            </Slider>
        </Box>
    );
};

export default ImageSlider;
