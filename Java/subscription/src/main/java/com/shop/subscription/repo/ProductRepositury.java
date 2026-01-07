package com.shop.subscription.repo;

import com.shop.subscription.models.Product;
import org.springframework.data.repository.CrudRepository;

public interface ProductRepositury extends CrudRepository<Product, Long> {

}
