package com.shop.subscription.repository;

import com.shop.subscription.entity.Product;
import org.springframework.data.repository.CrudRepository;

public interface ProductRepositury extends CrudRepository<Product, Long> {

}
