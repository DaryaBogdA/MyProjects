package com.shop.subscription.repo;

import com.shop.subscription.models.Users;
import org.springframework.data.repository.CrudRepository;

public interface UsersRepository extends CrudRepository<Users, Long> {
}
