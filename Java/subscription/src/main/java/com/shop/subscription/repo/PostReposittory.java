package com.shop.subscription.repo;

import com.shop.subscription.models.Post;
import org.springframework.data.repository.CrudRepository;

public interface PostReposittory extends CrudRepository<Post, Long> {

}
