import React from 'react';
import { useNavigate } from 'react-router-dom';

const HomePage: React.FC = () => {
    const navigate = useNavigate();

    return (
            <div className="main_layout">

                <div className="container">
                    <div className="row">
                        <div className="col-md-12 mt-4 mb-4">
                            <div id="carouselExample" className="carousel slide">
                                <div className="carousel-inner">
                                    <div className="carousel-item active">
                                        <img src="https://picsum.photos/1200/600" className="d-block w-100" alt="..."/>
                                    </div>
                                    <div className="carousel-item">
                                        <img src="https://picsum.photos/1200/599" className="d-block w-100" alt="..."/>
                                    </div>
                                    <div className="carousel-item">
                                        <img src="https://picsum.photos/1200/601" className="d-block w-100" alt="..."/>
                                    </div>
                                </div>
                                <button className="carousel-control-prev" type="button"
                                        data-bs-target="#carouselExample" data-bs-slide="prev">
                                    <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                                    <span className="visually-hidden">Previous</span>
                                </button>
                                <button className="carousel-control-next" type="button"
                                        data-bs-target="#carouselExample" data-bs-slide="next">
                                    <span className="carousel-control-next-icon" aria-hidden="true"></span>
                                    <span className="visually-hidden">Next</span>
                                </button>
                            </div>
                        </div>

                        <div className="row my-5">
                            <div className="col-md-4">
                                <h2 className="text-center">Hizmetler</h2>
                                <p className="text-center">Lorem ipsum dolor sit amet consectetur adipisicing elit.
                                    Accusamus explicabo odit ratione porro quaerat hic! Esse consectetur ipsum aut
                                    distinctio!</p>
                                <a className="text-center d-block" href="#">Devamı</a>
                            </div>
                            <div className="col-md-4">
                                <h2 className="text-center">Ulaşım</h2>
                                <p className="text-center">Lorem ipsum dolor sit amet consectetur adipisicing elit.
                                    Deserunt fugiat hic tempora ullam asperiores iusto. Officiis aliquid culpa
                                    doloremque similique!</p>
                                <a className="text-center d-block" href="#">Devamı</a>
                            </div>
                            <div className="col-md-4">
                                <h2 className="text-center">İletişim</h2>
                                <p className="text-center">Lorem ipsum dolor sit amet consectetur adipisicing elit. In
                                    aut ut dolores doloribus id voluptatem eveniet voluptates nisi ipsam iste?</p>
                                <a className="text-center d-block" href="#">Devamı</a>
                            </div>
                        </div>
                    </div>

                </div>

                <div className="gri-zemin">
                    <div className="container">
                        <div className="row mt-5 mb-5 py-10">
                            <div className="col-md-4 offset-md-2 text-center">
                                <img src="https://picsum.photos/375/430" className="rounded-5"/>
                            </div>
                            <div className="col-md-4">
                                <p className="fs-1 fw-semibold">GOSB<br/>TEKNOPARK</p>
                                <p className="fs-5">Lorem ipsum dolor sit, amet consectetur adipisicing elit. Rem
                                    placeat id accusamus sit ducimus non repellat porro alias incidunt illum.</p>
                                <p>Lorem ipsum dolor sit, amet consectetur adipisicing elit. Rem placeat id accusamus
                                    sit ducimus non repellat porro alias incidunt illum.</p>
                                <a href="#" className="btn btn-secondary">Devamını oku</a>
                            </div>
                        </div>
                    </div>
                </div>


                <div className="container">
                    <div className="row blog">
                        <h2 className="text-center">BLOG</h2>
                        <p className="text-center">Lorem, ipsum dolor sit amet consectetur adipisicing elit. Aliquam,
                            maxime?</p>
                        <div className="col-md-4">
                            <div className="card">
                                <img src="https://picsum.photos/300/200"/>
                                <div className="card-body">
                                    <h5 className="card-title">HTML'in Temelleri</h5>
                                    <p className="card-text">Some quick example text to build on the card title and make
                                        up the bulk of the card’s content.</p>
                                    <a href="#" className="btn btn-primary">Yazıyı Oku</a>
                                </div>
                            </div>
                        </div>
                        <div className="col-md-4">
                            <div className="card">
                                <img src="https://picsum.photos/300/200"/>
                                <div className="card-body">
                                    <h5 className="card-title">HTML'in Temelleri</h5>
                                    <p className="card-text">Some quick example text to build on the card title and make
                                        up the bulk of the card’s content.</p>
                                    <a href="#" className="btn btn-primary">Yazıyı Oku</a>
                                </div>
                            </div>
                        </div>
                        <div className="col-md-4">
                            <div className="card">
                                <img src="https://picsum.photos/300/200"/>
                                <div className="card-body">
                                    <h5 className="card-title">HTML'in Temelleri</h5>
                                    <p className="card-text">Some quick example text to build on the card title and make
                                        up the bulk of the card’s content.</p>
                                    <a href="#" className="btn btn-primary">Yazıyı Oku</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

    );
};

export default HomePage;
