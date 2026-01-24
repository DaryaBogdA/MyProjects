package com.shop.subscription.repository;

import com.shop.subscription.entity.Users;
import org.springframework.data.repository.CrudRepository;

public interface UsersRepository extends CrudRepository<Users, Long> {
}
